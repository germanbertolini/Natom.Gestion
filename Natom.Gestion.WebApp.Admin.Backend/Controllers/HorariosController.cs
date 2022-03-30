using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.Core.Biz.Entities.Models;
using Natom.Gestion.Core.Biz.Managers;
using Natom.Extensions.Auth.Attributes;
using Natom.Gestion.WebApp.Admin.Backend.DTO;
using Natom.Gestion.WebApp.Admin.Backend.DTO.DataTable;
using Natom.Gestion.WebApp.Admin.Backend.DTO.Horarios;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HorariosController : BaseController
    {
        public HorariosController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: horarios/list?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_clientes_places_horarios")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string encryptedId)
        {
            try
            {
                var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedId));

                var manager = new HorariosManager(_serviceProvider);
                var horariosCount = await manager.ObtenerCountAsync(placeId);
                var horarios = await manager.ObtenerDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, placeId);

                return Ok(new ApiResultDTO<DataTableResponseDTO<HorarioDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<HorarioDTO>
                    {
                        RecordsTotal = horariosCount,
                        RecordsFiltered = horarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = horarios.Select(goal => new HorarioDTO().From(goal)).ToList()
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

        // GET: horarios/basics/data
        // GET: horarios/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_clientes_places_horarios")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedPlaceId, [FromQuery] string encryptedClientId, [FromQuery] string encryptedId = null)
        {
            try
            {
                var clientId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(encryptedClientId));

                var manager = new HorariosManager(_serviceProvider);
                HorarioDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var horarioId = EncryptionService.Decrypt<int, ConfigTolerancia>(Uri.UnescapeDataString(encryptedId));
                    var horario = await manager.ObtenerAsync(horarioId);
                    entity = new HorarioDTO().From(horario);
                }
                else
                {
                    var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedPlaceId));
                    var horario = await manager.ObtenerVigenteAsync(clientId, placeId);

                    //VALORES POR DEFAULT PRIMER CONFIGURACIÓN
                    if (horario == null)
                    {
                        horario = new ConfigTolerancia()
                        {
                            IngresoToleranciaMins = 10,
                            EgresoToleranciaMins = 10,
                            AlmuerzoHorarioDesde = new TimeSpan(12, 0, 0),
                            AlmuerzoHorarioHasta = new TimeSpan(15, 0, 0),
                            AlmuerzoTiempoLimiteMins = 65,
                            AplicaDesde = DateTime.Now.Date
                        };
                    }
                    horario.ConfigToleranciaId = 0;
                    entity = new HorarioDTO().From(horario);
                }

                return Ok(new ApiResultDTO<HorarioDTO>
                {
                    Success = true,
                    Data = entity
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

        // POST: horarios/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_clientes_places_horarios")]
        public async Task<IActionResult> PostSaveAsync([FromBody] HorarioDTO horarioDto, [FromQuery] string encryptedClientId)
        {
            try
            {
                var clientId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(encryptedClientId));

                var manager = new HorariosManager(_serviceProvider);
                var horario = await manager.GuardarAsync(clientId, _accessToken.UserId ?? -1, horarioDto.ToModel(clientId, horarioDto));

                return Ok(new ApiResultDTO<HorarioDTO>
                {
                    Success = true,
                    Data = new HorarioDTO().From(horario)
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
