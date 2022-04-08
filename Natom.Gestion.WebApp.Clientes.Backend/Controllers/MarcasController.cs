using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Marcas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MarcasController : BaseController
    {
        public MarcasController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: marcas/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new MarcasManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerMarcasCountAsync();
                var usuarios = await manager.ObtenerMarcasDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<MarcaDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<MarcaDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new MarcaDTO().From(usuario)).ToList()
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

        // GET: marcas/basics/data
        // GET: marcas/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new MarcasManager(_serviceProvider);
                MarcaDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var marcaId = EncryptionService.Decrypt<int, Marca>(Uri.UnescapeDataString(encryptedId));
                    var marca = await manager.ObtenerMarcaAsync(marcaId);
                    entity = new MarcaDTO().From(marca);
                }

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity
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

        // POST: marcas/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] MarcaDTO marcaDto)
        {
            try
            {
                var manager = new MarcasManager(_serviceProvider);
                var marca = await manager.GuardarMarcaAsync(marcaDto);

                await RegistrarAccionAsync(marca.MarcaId, nameof(Marca), string.IsNullOrEmpty(marcaDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<MarcaDTO>
                {
                    Success = true,
                    Data = new MarcaDTO().From(marca)
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

        // DELETE: marcas/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var marcaId = EncryptionService.Decrypt<int, Marca>(Uri.UnescapeDataString(encryptedId));

                var manager = new MarcasManager(_serviceProvider);
                await manager.DesactivarMarcaAsync(marcaId);

                await RegistrarAccionAsync(marcaId, nameof(Marca), "Baja");

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

        // POST: marcas/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var marcaId = EncryptionService.Decrypt<int, Marca>(Uri.UnescapeDataString(encryptedId));

                var manager = new MarcasManager(_serviceProvider);
                await manager.ActivarMarcaAsync(marcaId);

                await RegistrarAccionAsync(marcaId, nameof(Marca), "Alta");

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
    }
}
