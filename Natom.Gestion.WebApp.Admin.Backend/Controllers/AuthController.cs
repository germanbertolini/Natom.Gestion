using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Extensions.Auth.Entities.Models;
using Natom.Extensions.Auth.Services;
using Natom.Gestion.WebApp.Admin.Backend.DTO;
using Natom.Gestion.WebApp.Admin.Backend.DTO.Auth;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : BaseController
    {
        private readonly AuthService _authService;

        public AuthController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
        }

        // POST: auth/login
        [HttpPost]
        [ActionName("login")]
        public async Task<IActionResult> PostLogin()
        {
            try
            {
                string email, password;

                GetClientAndSecretFromAuthorizationBasic(out email, out password);

                Usuario usuario = null;
                var permissions = new List<string>();

                var usuarioAdmin = await _configurationService.GetValueAsync("WebApp.Admin.Authentication.Admin.UserName");
                if (email.ToLower().Equals(usuarioAdmin.ToLower()))
                {
                    var usuarioClave = await _configurationService.GetValueAsync("WebApp.Admin.Authentication.Admin.Password");
                    if (!EncryptionService.CreateMD5(usuarioClave).Equals(EncryptionService.CreateMD5(password)))
                        throw new HandledException("Usuario y/o clave inválida");
                    usuario = new Usuario
                    {
                        Nombre = "Admin",
                        ClienteNombre = "Natom",
                        Email = usuarioAdmin,
                        FechaHoraAlta = DateTime.Now
                    };
                    permissions.Add("abm_usuarios");
                }
                else
                {
                    usuario = await _authService.AuthenticateUserAsync(email, password);
                    permissions = usuario != null ? usuario.Permisos.Select(p => p.PermisoId).ToList() : new List<string>();
                }

                var tokenDurationMinutes = Convert.ToInt32(await _configurationService.GetValueAsync("WebApp.Admin.Authentication.TokenDurationMins"));
                var authToken = await _authService.CreateTokenAsync(userId: usuario.UsuarioId, userName: $"{usuario.Nombre} {usuario.Apellido}", clientId: usuario.ClienteId, clientName: usuario.ClienteNombre, permissions, tokenDurationMinutes);


                return Ok(new ApiResultDTO<LoginResultDTO>
                {
                    Success = true,
                    Data = new LoginResultDTO
                    {
                        User = new UserDTO().From(usuario),
                        Permissions = permissions,
                        Token = authToken.ToJwtEncoded()
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: auth/logout
        [HttpPost]
        [ActionName("logout")]
        public async Task<IActionResult> PostLogout()
        {
            try
            {
                if (_accessToken.UserId.HasValue)
                    await _authService.DestroyTokenAsync(_accessToken.UserId.Value, scope: "WebApp.Admin");

                return Ok(new ApiResultDTO
                {
                    Success = true
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        private void GetClientAndSecretFromAuthorizationBasic(out string client, out string secret)
        {
            client = null;
            secret = null;

            string authorization = GetAuthorizationFromHeader();
            if (!authorization.StartsWith("Basic")) throw new HandledException("Authorization header inválido");

            var data = Convert.FromBase64String(authorization.Replace("Basic ", ""));
            var authorizationDecoded = Encoding.UTF8.GetString(data);

            if (authorizationDecoded.IndexOf(":") >= 0)
            {
                client = authorizationDecoded.Substring(0, authorizationDecoded.IndexOf(":"));
                secret = authorizationDecoded.Substring(authorizationDecoded.IndexOf(":") + 1);
            }
            else
            {
                throw new HandledException("No se pudo obtener el client y secret");
            }
        }
    }
}
