using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Pedidos;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RangosHorariosController : BaseController
    {
        public RangosHorariosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: rangoshorarios/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new RangosHorariosManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerRangosHorariosCountAsync();
                var usuarios = await manager.ObtenerRangosHorariosDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<RangoHorarioDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<RangoHorarioDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new RangoHorarioDTO().From(usuario)).ToList()
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

        // GET: rangoshorarios/basics/data
        // GET: rangoshorarios/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new RangosHorariosManager(_serviceProvider);
                RangoHorarioDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var rangoHorarioId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var rangoHorario = await manager.ObtenerRangoHorarioAsync(rangoHorarioId);
                    entity = new RangoHorarioDTO().From(rangoHorario);
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

        // POST: rangoshorarios/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] RangoHorarioDTO rangoHorarioDto)
        {
            try
            {
                var manager = new RangosHorariosManager(_serviceProvider);
                var rangoHorario = await manager.GuardarRangoHorarioAsync(rangoHorarioDto);

                await RegistrarAccionAsync(rangoHorario.RangoHorarioId, nameof(RangoHorario), string.IsNullOrEmpty(rangoHorarioDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<RangoHorarioDTO>
                {
                    Success = true,
                    Data = new RangoHorarioDTO().From(rangoHorario)
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

        // DELETE: rangoshorarios/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var rangoHorarioId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new RangosHorariosManager(_serviceProvider);
                await manager.DesactivarRangoHorarioAsync(rangoHorarioId);

                await RegistrarAccionAsync(rangoHorarioId, nameof(RangoHorario), "Baja");

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

        // POST: rangoshorarios/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var rangoHorarioId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new RangosHorariosManager(_serviceProvider);
                await manager.ActivarRangoHorarioAsync(rangoHorarioId);

                await RegistrarAccionAsync(rangoHorarioId, nameof(RangoHorario), "Alta");

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
