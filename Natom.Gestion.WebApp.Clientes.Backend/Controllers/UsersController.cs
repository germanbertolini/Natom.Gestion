using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Auth;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Natom.Extensions.Auth.Repository;
using Natom.Extensions.Auth.Services;
using Natom.Extensions.Auth.Entities.Models;
using Natom.Extensions.Logger.Entities;
using Natom.Extensions.Auth.Attributes;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsersController : BaseController
    {
        private readonly AuthService _authService;
        
        public UsersController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
        }

        // POST: users/list
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody]DataTableRequestDTO request)
        {
            try
            {
                if ((_accessToken.ClientId ?? 0) == 0)
                    throw new HandledException("El administrador de Natom solamente puede visualizar y administrar los usuarios del cliente desde la aplicación de -Admin-");

                var repository = new UsuarioRepository(_serviceProvider);
                var usuarios = await repository.ListByClienteAndScopeAsync(scope: "WebApp.Clientes", request.Search.Value, request.Start, request.Length, _accessToken.ClientId ?? -1);

                int thisUsuarioId = _accessToken.UserId ?? -1;

                return Ok(new ApiResultDTO<DataTableResponseDTO<UserDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<UserDTO>
                    {
                        RecordsTotal = usuarios.FirstOrDefault()?.TotalRegistros ?? 0,
                        RecordsFiltered = usuarios.FirstOrDefault()?.TotalFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new UserDTO().From(usuario, thisUsuarioId)).ToList()
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

        // GET: users/basics/data
        // GET: users/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery]string encryptedId = null)
        {
            try
            {
                var repository = new UsuarioRepository(_serviceProvider);
                var permisos = await repository.ListPermisosAsync(scope: "WebApp.Clientes");
                UserDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var usuarioId = EncryptionService.Decrypt<int, Usuario>(Uri.UnescapeDataString(encryptedId));
                    var usuario = await repository.ObtenerUsuarioAsync(usuarioId);
                    entity = new UserDTO().From(usuario);
                }

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        permisos = permisos.Select(permiso => new PermisoDTO().From(permiso)).ToList()
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

        // POST: users/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_usuarios")]
        public async Task<IActionResult> PostSaveAsync([FromBody] UserDTO user)
        {
            try
            {
                if ((_accessToken.ClientId ?? 0) == 0)
                    throw new HandledException("El administrador de Natom solamente puede generar los usuarios del cliente desde la aplicación de -Admin-");

                var manager = new UsuarioRepository(_serviceProvider);
                var model = user.ToModel(scope: "WebApp.Clientes");
                var isNew = model.UsuarioId == 0;
                var secretConfirmation = isNew ? Guid.NewGuid().ToString("N") : "";
                var usuario = await manager.GuardarUsuarioAsync(scope: "WebApp.Clientes", model, secretConfirmation, (_accessToken.UserId ?? 0));

                if (isNew)
                    await EnviarEmailParaConfirmarRegistroAsync(_transaction, scope: "WebApp.Clientes", usuario);

                return Ok(new ApiResultDTO<UserDTO>
                {
                    Success = true,
                    Data = new UserDTO().From(usuario)
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

        // DELETE: users/delete?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("delete")]
        [TienePermiso(Permiso = "abm_usuarios")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var usuarioId = EncryptionService.Decrypt<int, Usuario>(Uri.UnescapeDataString(encryptedId));

                var manager = new UsuarioRepository(_serviceProvider);
                await manager.EliminarUsuarioAsync(usuarioId, (_accessToken.UserId ?? 0));

                await _authService.DestroyTokensByUsuarioIdAsync(usuarioId);

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

        // POST: users/confirm?data={data}
        [HttpPost]
        [ActionName("confirm")]
        public async Task<IActionResult> ConfirmAsync([FromQuery] string data)
        {
            try
            {
                var encodedString = Uri.UnescapeDataString(data);
                byte[] dataBytes = Convert.FromBase64String(encodedString);
                string dataString = Encoding.UTF8.GetString(dataBytes);
                var obj = JsonConvert.DeserializeObject<dynamic>(dataString);
                var secret = (string)obj.s;
                var clave = (string)obj.p;

                var manager = new UsuarioRepository(_serviceProvider);
                await manager.ConfirmarUsuarioAsync(secret, EncryptionService.CreateMD5(clave));

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

        // POST: users/recover?email={email}
        [HttpPost]
        [ActionName("recover")]
        public async Task<IActionResult> RecoverAsync([FromQuery] string email)
        {
            try
            {
                var manager = new UsuarioRepository(_serviceProvider);
                var secretConfirmation = Guid.NewGuid().ToString("N");
                var usuario = await manager.RecuperarUsuarioByEmailAsync(scope: "WebApp.Clientes", Uri.UnescapeDataString(email), secretConfirmation, (_accessToken.UserId ?? 0));

                if (usuario.FechaHoraUltimoEmailEnviado.HasValue && usuario.FechaHoraUltimoEmailEnviado.Value.AddMinutes(10) > DateTime.Now)
                    throw new HandledException("Se ha enviado un mail de recuperación de clave hace menos de 10 minutos. Aguarde unos minutos y vuelva a intentarlo.");

                await EnviarEmailParaRecuperarClaveAsync(_transaction, scope: "WebApp.Clientes", usuario);

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

        // POST: users/recover_by_id?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("recover_by_id")]
        [TienePermiso(Permiso = "abm_usuarios")]
        public async Task<IActionResult> RecoverByIdAsync([FromQuery] string encryptedId)
        {
            try
            {
                var usuarioId = EncryptionService.Decrypt<int, Usuario>(Uri.UnescapeDataString(encryptedId));

                var manager = new UsuarioRepository(_serviceProvider);
                var secretConfirmation = Guid.NewGuid().ToString("N");
                var usuario = await manager.RecuperarUsuarioAsync(scope: "WebApp.Clientes", usuarioId, secretConfirmation, usuarioId);

                if (usuario.FechaHoraUltimoEmailEnviado.HasValue && usuario.FechaHoraUltimoEmailEnviado.Value.AddMinutes(10) > DateTime.Now)
                    throw new HandledException("Se ha enviado un mail de recuperación de clave hace menos de 10 minutos. Aguarde unos minutos y vuelva a intentarlo.");

                await EnviarEmailParaRecuperarClaveAsync(_transaction, scope: "WebApp.Admin", usuario);

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

        private async Task EnviarEmailParaConfirmarRegistroAsync(Transaction transaction, string scope, Usuario usuario)
        {
            string subject = "Confirmar registración";
            string appAddress = await _configurationService.GetValueAsync($"{scope}.URL");
            string productName = await _configurationService.GetValueAsync("General.ProductName");
            var dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { s = usuario.SecretConfirmacion, e = usuario.Email }));
            var data = Uri.EscapeDataString(Convert.ToBase64String(dataBytes));
            string link = new Uri($"{appAddress}/users/confirm/{data}").AbsoluteUri;
            string body = System.IO.File.ReadAllText("EmailTemplates/Default.html");
            body = body.Replace("$body$", String.Format("<h2>¡Bienvenido a " + productName + "!</h2><br/><p>Por favor, para <b>generar la clave de acceso al sistema</b> haga clic en el siguiente link: <a href='{0}'>{0}</a></p>", link));
            await _mailService.EnviarMailAsync(transaction, subject, body, usuario.Email, usuario.Nombre);
        }

        private async Task EnviarEmailParaRecuperarClaveAsync(Transaction transaction, string scope, Usuario usuario)
        {
            string subject = "Recuperar clave";
            string appAddress = await _configurationService.GetValueAsync($"{scope}.URL");
            string productName = await _configurationService.GetValueAsync("General.ProductName");
            var dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { s = usuario.SecretConfirmacion, e = usuario.Email }));
            var data = Uri.EscapeDataString(Convert.ToBase64String(dataBytes));
            string link = new Uri($"{appAddress}/users/confirm/{data}").AbsoluteUri;
            string body = System.IO.File.ReadAllText("EmailTemplates/Default.html");
            body = body.Replace("$body$", String.Format("<h2>Recupero de clave " + productName + "</h2><br/><p>Por favor, para <b>recuperar la clave de acceso al sistema</b> haga clic en el siguiente link: <a href='{0}'>{0}</a></p>", link));
            await _mailService.EnviarMailAsync(transaction, subject, body, usuario.Email, usuario.Nombre);
        }
    }
}
