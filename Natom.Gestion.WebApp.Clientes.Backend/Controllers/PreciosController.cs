using Microsoft.AspNetCore.Mvc;
using Natom.Gestion.WebApp.Clientes.Backend.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Marcas;
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
    public class PreciosController : BaseController
    {
        public PreciosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: precios/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string lista = null)
        {
            try
            {
                int? listaDePreciosId = null;
                if (!string.IsNullOrEmpty(lista))
                    listaDePreciosId = EncryptionService.Decrypt<int>(lista);

                var manager = new PreciosManager(_serviceProvider);
                var precios = manager.ObtenerPreciosDataTable(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, listaDePreciosId);

                var listasDePrecios = await manager.ObtenerListasDePreciosAsync();

                return Ok(new ApiResultDTO<DataTableResponseDTO<PrecioListDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<PrecioListDTO>
                    {
                        RecordsTotal = precios.FirstOrDefault()?.CantidadRegistros ?? 0,
                        RecordsFiltered = precios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = precios.Select(precio => new PrecioListDTO().From(precio)).ToList(),
                        ExtraData = new
                        {
                            listasDePrecios = listasDePrecios.Select(lista => new ListaDePreciosDTO().From(lista))
                        }
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: precios/basics/data
        // GET: precios/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new PreciosManager(_serviceProvider);
                PrecioDTO entity = null;

                var listasDePrecios = await manager.ObtenerListasDePreciosAsync();

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var productoPrecioId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var precio = await manager.ObtenerPrecioAsync(productoPrecioId);
                    entity = new PrecioDTO().From(precio);

                    //BUSCAMOS LOS PRECIOS ACTUALES DEL PRODUCTO
                    var preciosActuales = manager.ObtenerPreciosActuales(precio.ProductoId);
                    preciosActuales.ForEach(p =>
                    {
                        var lista = listasDePrecios.FirstOrDefault(l => l.ListaDePreciosId.Equals(p.ListaDePreciosId));

                        if (p.ProductoPrecioId.HasValue && lista.EsPorcentual) //SI LA LISTA ES PORCENTUAL PERO EL PRECIO YA FUE DEFINIDO ENTONCES MARCAMOS LA LISTA COMO -NO PORCENTUAL-
                            lista.EsPorcentual = false;
                    });
                }
                                

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        listasDePrecios = listasDePrecios.Select(lista => new ListaDePreciosDTO().From(lista))
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: precios/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] PrecioDTO precioDto)
        {
            try
            {
                var manager = new PreciosManager(_serviceProvider);
                var precio = await manager.GuardarPrecioAsync(precioDto);

                await RegistrarAccionAsync(precio.ProductoPrecioId, nameof(ProductoPrecio), string.IsNullOrEmpty(precioDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<PrecioDTO>
                {
                    Success = true,
                    Data = new PrecioDTO().From(precio)
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: precios/reajustes/list?status={status}&lista={lista}
        [HttpPost]
        [ActionName("reajustes/list")]
        public async Task<IActionResult> PostReajustesListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null, [FromQuery] string lista = null)
        {
            try
            {
                var manager = new PreciosManager(_serviceProvider);
                var preciosCount = await manager.ObtenerPreciosReajustesCountAsync();
                var precios = await manager.ObtenerPreciosReajustesDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status, lista);

                var listasDePrecios = await manager.ObtenerListasDePreciosAsync();

                return Ok(new ApiResultDTO<DataTableResponseDTO<PrecioReajusteListDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<PrecioReajusteListDTO>
                    {
                        RecordsTotal = preciosCount,
                        RecordsFiltered = precios.Count,
                        Records = precios.Select(precio => new PrecioReajusteListDTO().From(precio)).ToList(),
                        ExtraData = new
                        {
                            listasDePrecios = listasDePrecios.Select(lista => new ListaDePreciosDTO().From(lista))
                        }
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: precios/reajustes/basics/data
        // GET: precios/reajustes/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("reajustes/basics/data")]
        public async Task<IActionResult> GetReajustesBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new PreciosManager(_serviceProvider);
                PrecioReajusteDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var marcaId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var precioReajuste = await manager.ObtenerPreciosReajusteAsync(marcaId);
                    entity = new PrecioReajusteDTO().From(precioReajuste);
                }

                var listasDePrecios = await manager.ObtenerListasDePreciosAsync();
                var marcas = await new MarcasManager(_serviceProvider).ObtenerMarcasActivasAsync();

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        listasDePrecios = listasDePrecios.Select(lista => new ListaDePreciosDTO().From(lista)),
                        marcas = marcas.Select(marca => new MarcaDTO().From(marca))
                    }
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: precios/reajustes/save
        [HttpPost]
        [ActionName("reajustes/save")]
        public async Task<IActionResult> PostReajustesSaveAsync([FromBody] PrecioReajusteDTO precioReajusteDto)
        {
            try
            {
                var manager = new PreciosManager(_serviceProvider);
                var reajuste = await manager.GuardarReajustePrecioAsync((int)(_token?.UserId ?? 0), precioReajusteDto);

                await RegistrarAccionAsync(reajuste.HistoricoReajustePrecioId, nameof(HistoricoReajustePrecio), string.IsNullOrEmpty(precioReajusteDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<PrecioReajusteDTO>
                {
                    Success = true,
                    Data = new PrecioReajusteDTO().From(reajuste)
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // DELETE: precios/reajustes/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("reajustes/disable")]
        public async Task<IActionResult> DeleteReajustesAsync([FromQuery] string encryptedId)
        {
            try
            {
                var reajusteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new PreciosManager(_serviceProvider);
                await manager.EliminarReajusteAsync(reajusteId);

                await RegistrarAccionAsync(reajusteId, nameof(HistoricoReajustePrecio), "Baja");

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
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: precios/get?producto={encryptedProductoId}&lista={encryptedDepositoId}
        [HttpGet]
        [ActionName("get")]
        public async Task<IActionResult> GetPrecioActualAsync([FromQuery] string producto, [FromQuery] string lista)
        {
            try
            {
                int productoId = EncryptionService.Decrypt<int>(producto);
                int listaDePreciosId = 0;

                if (!string.IsNullOrEmpty(lista))
                    listaDePreciosId = EncryptionService.Decrypt<int>(lista);
                else
                    throw new HandledException("Falta la Lista de precios");

                var manager = new PreciosManager(_serviceProvider);
                var precioActual = manager.ObtenerPrecioActual(productoId, listaDePreciosId);

                return Ok(new ApiResultDTO<decimal?>
                {
                    Success = true,
                    Data = precioActual
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                await LoggingService.LogExceptionAsync(_db, ex, usuarioId: (int)(_token?.UserId ?? 0), _userAgent);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }
    }
}
