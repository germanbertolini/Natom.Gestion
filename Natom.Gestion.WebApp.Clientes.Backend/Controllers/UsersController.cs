using Microsoft.AspNetCore.Mvc;
using Natom.Gestion.WebApp.Clientes.Backend.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Auth;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsersController : BaseController
    {
        public UsersController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: users/list
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody]DataTableRequestDTO request)
        {
            try
            {
                var manager = new UsuariosManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerUsuariosCountAsync();
                var usuarios = await manager.ObtenerUsuariosDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction);

                return Ok(new ApiResultDTO<DataTableResponseDTO<UserDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<UserDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new UserDTO().From(usuario, manager.ObtenerEstado(usuario))).ToList()
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
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
                var manager = new UsuariosManager(_serviceProvider);
                var permisos = await manager.ObtenerListaPermisosAsync();
                UserDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var usuarioId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var usuario = await manager.ObtenerUsuarioAsync(usuarioId);
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
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: users/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] UserDTO user)
        {
            try
            {
                var manager = new UsuariosManager(_serviceProvider);
                var usuario = await manager.GuardarUsuarioAsync(_configuration, user);

                await RegistrarAccionAsync(usuario.UsuarioId, nameof(Usuario), string.IsNullOrEmpty(user.EncryptedId) ? "Alta" : "Edición");

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
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // DELETE: users/delete?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("delete")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var usuarioId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new UsuariosManager(_serviceProvider);
                await manager.EliminarUsuarioAsync(usuarioId);

                await RegistrarAccionAsync(usuarioId, nameof(Usuario), "Baja");

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
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
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

                var manager = new UsuariosManager(_serviceProvider);
                await manager.ConfirmarUsuarioAsync(secret, clave);

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
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: users/recover?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("recover")]
        public async Task<IActionResult> RecoverAsync([FromQuery] string encryptedId)
        {
            try
            {
                var usuarioId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new UsuariosManager(_serviceProvider);
                await manager.RecuperarUsuarioAsync(_configuration, usuarioId);

                await RegistrarAccionAsync(usuarioId, nameof(Usuario), "Solicita recuperación de clave");

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
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int?)_token?.UserId, _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }
    }
}
