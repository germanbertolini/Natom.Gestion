using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Transportes;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TransportesController : BaseController
    {
        public TransportesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: transportes/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new TransportesManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerTransportesCountAsync();
                var usuarios = await manager.ObtenerTransportesDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<TransporteDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<TransporteDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new TransporteDTO().From(usuario)).ToList()
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

        // GET: transportes/basics/data
        // GET: transportes/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new TransportesManager(_serviceProvider);
                TransporteDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var transporteId = EncryptionService.Decrypt<int, Transporte>(Uri.UnescapeDataString(encryptedId));
                    var transporte = await manager.ObtenerTransporteAsync(transporteId);
                    entity = new TransporteDTO().From(transporte);
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

        // POST: transportes/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] TransporteDTO transporteDto)
        {
            try
            {
                var manager = new TransportesManager(_serviceProvider);
                var transporte = await manager.GuardarTransporteAsync(transporteDto);

                await RegistrarAccionAsync(transporte.TransporteId, nameof(Transporte), string.IsNullOrEmpty(transporteDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<TransporteDTO>
                {
                    Success = true,
                    Data = new TransporteDTO().From(transporte)
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

        // DELETE: transportes/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var transporteId = EncryptionService.Decrypt<int, Transporte>(Uri.UnescapeDataString(encryptedId));

                var manager = new TransportesManager(_serviceProvider);
                await manager.DesactivarTransporteAsync(transporteId);

                await RegistrarAccionAsync(transporteId, nameof(Transporte), "Baja");

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

        // POST: transportes/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var transporteId = EncryptionService.Decrypt<int, Transporte>(Uri.UnescapeDataString(encryptedId));

                var manager = new TransportesManager(_serviceProvider);
                await manager.ActivarTransporteAsync(transporteId);

                await RegistrarAccionAsync(transporteId, nameof(Transporte), "Alta");

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
