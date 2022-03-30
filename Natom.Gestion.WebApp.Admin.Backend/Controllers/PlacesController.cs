using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.Core.Biz.Entities.Models;
using Natom.Gestion.Core.Biz.Managers;
using Natom.Extensions.Auth.Attributes;
using Natom.Gestion.WebApp.Admin.Backend.DTO;
using Natom.Gestion.WebApp.Admin.Backend.DTO.DataTable;
using Natom.Gestion.WebApp.Admin.Backend.DTO.Places;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PlacesController : BaseController
    {
        public PlacesController(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // POST: places/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        [TienePermiso(Permiso = "abm_clientes_places_goals")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string encryptedId, [FromQuery] string status = null)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(encryptedId));

                var manager = new PlacesManager(_serviceProvider);
                var placesCount = await manager.ObtenerCountAsync(clienteId);
                var places = await manager.ObtenerDataTableAsync(clienteId, request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<PlaceDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<PlaceDTO>
                    {
                        RecordsTotal = placesCount,
                        RecordsFiltered = places.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = places.Select(place => new PlaceDTO().From(place)).ToList()
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

        // GET: places/basics/data
        // GET: places/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        [TienePermiso(Permiso = "abm_clientes_places_goals")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new PlacesManager(_serviceProvider);
                PlaceDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedId));
                    var place = await manager.ObtenerAsync(placeId);
                    entity = new PlaceDTO().From(place);
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

        // POST: places/save
        [HttpPost]
        [ActionName("save")]
        [TienePermiso(Permiso = "abm_clientes_places_goals")]
        public async Task<IActionResult> PostSaveAsync([FromBody] PlaceDTO placeDto, [FromQuery] string encryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(encryptedId));

                var manager = new PlacesManager(_serviceProvider);
                var place = await manager.GuardarAsync(placeDto.ToModel(clienteId));

                await RegistrarAccionAsync(clienteId, place.PlaceId, nameof(Place), string.IsNullOrEmpty(placeDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<PlaceDTO>
                {
                    Success = true,
                    Data = new PlaceDTO().From(place)
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

        // DELETE: places/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        [TienePermiso(Permiso = "abm_clientes_places_goals")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId, [FromQuery] string clienteEncryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(clienteEncryptedId));
                var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedId));

                var manager = new PlacesManager(_serviceProvider);
                await manager.DesactivarAsync(placeId);

                await RegistrarAccionAsync(clienteId, placeId, nameof(Place), "Baja");

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

        // POST: places/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        [TienePermiso(Permiso = "abm_clientes_places_goals")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId, [FromQuery] string clienteEncryptedId)
        {
            try
            {
                var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(clienteEncryptedId));
                var placeId = EncryptionService.Decrypt<int, Place>(Uri.UnescapeDataString(encryptedId));

                var manager = new PlacesManager(_serviceProvider);
                await manager.ActivarAsync(placeId);

                await RegistrarAccionAsync(clienteId, placeId, nameof(Place), "Alta");

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
