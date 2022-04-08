using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Precios;
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
    public class ListasDePreciosController : BaseController
    {
        public ListasDePreciosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: listasdeprecios/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new ListasDePreciosManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerListasDePreciosCountAsync();
                var usuarios = await manager.ObtenerListasDePreciosDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<ListaDePreciosDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<ListaDePreciosDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new ListaDePreciosDTO().From(usuario)).ToList()
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

        // GET: listasdeprecios/basics/data
        // GET: listasdeprecios/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new ListasDePreciosManager(_serviceProvider);
                ListaDePreciosDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var listaDePreciosId = EncryptionService.Decrypt<int, ListaDePrecios>(Uri.UnescapeDataString(encryptedId));
                    var listaDePrecios = await manager.ObtenerListaDePrecioAsync(listaDePreciosId);
                    entity = new ListaDePreciosDTO().From(listaDePrecios);
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

        // POST: listasdeprecios/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] ListaDePreciosDTO listaDePreciosDto)
        {
            try
            {
                var manager = new ListasDePreciosManager(_serviceProvider);
                var listaDePrecios = await manager.GuardarListaDePrecioAsync(listaDePreciosDto);

                await RegistrarAccionAsync(listaDePrecios.ListaDePreciosId, nameof(ListaDePrecios), string.IsNullOrEmpty(listaDePreciosDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<ListaDePreciosDTO>
                {
                    Success = true,
                    Data = new ListaDePreciosDTO().From(listaDePrecios)
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

        // DELETE: listasdeprecios/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var listaDePreciosId = EncryptionService.Decrypt<int, ListaDePrecios>(Uri.UnescapeDataString(encryptedId));

                var manager = new ListasDePreciosManager(_serviceProvider);
                await manager.DesactivarListaDePrecioAsync(listaDePreciosId);

                await RegistrarAccionAsync(listaDePreciosId, nameof(ListaDePrecios), "Baja");

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

        // POST: listasdeprecios/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var listaDePreciosId = EncryptionService.Decrypt<int, ListaDePrecios>(Uri.UnescapeDataString(encryptedId));

                var manager = new ListasDePreciosManager(_serviceProvider);
                await manager.ActivarListaDePrecioAsync(listaDePreciosId);

                await RegistrarAccionAsync(listaDePreciosId, nameof(ListaDePrecios), "Alta");

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