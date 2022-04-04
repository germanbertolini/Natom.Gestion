using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Autocomplete;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Marcas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Productos;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductosController : BaseController
    {
        public ProductosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: productos/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new ProductosManager(_serviceProvider);
                var productosCount = await manager.ObtenerProductosCountAsync();
                var productos = await manager.ObtenerProductosDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<ProductoListDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<ProductoListDTO>
                    {
                        RecordsTotal = productosCount,
                        RecordsFiltered = productos.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = productos.Select(producto => new ProductoListDTO().From(producto)).ToList()
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

        // GET: productos/search?filter={filter}
        [HttpGet]
        [ActionName("search")]
        public async Task<IActionResult> GetSearchAsync([FromQuery] string filter = null)
        {
            try
            {
                var manager = new ProductosManager(_serviceProvider);
                var productos = await manager.BuscarProductosAsync(size: 20, filter);

                return Ok(new ApiResultDTO<AutocompleteResponseDTO<ProductoListDTO>>
                {
                    Success = true,
                    Data = new AutocompleteResponseDTO<ProductoListDTO>
                    {
                        Total = productos.Count,
                        Results = productos.Select(producto => new ProductoListDTO().From(producto)).ToList()
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

        // GET: productos/basics/data
        // GET: productos/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new ProductosManager(_serviceProvider);
                var marcasManager = new MarcasManager(_serviceProvider);
                ProductoDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var productoId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var producto = await manager.ObtenerProductoAsync(productoId);
                    entity = new ProductoDTO().From(producto);
                }

                var marcas = await marcasManager.ObtenerMarcasActivasAsync();
                var unidades = await manager.ObtenerUnidadesPesoAsync();
                var categorias = await manager.ObtenerCategoriasActivasAsync();

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        marcas = marcas.Select(marca => new MarcaDTO().From(marca)).ToList(),
                        unidades = unidades.Select(unidad => new UnidadPesoDTO().From(unidad)).ToList(),
                        categorias = categorias.Select(categoria => new CategoriaProductoDTO().From(categoria)).ToList()
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

        // POST: productos/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] ProductoDTO productoDto)
        {
            try
            {
                var manager = new ProductosManager(_serviceProvider);
                var producto = await manager.GuardarProductoAsync(productoDto);

                await RegistrarAccionAsync(producto.ProductoId, nameof(Producto), string.IsNullOrEmpty(productoDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<ProductoDTO>
                {
                    Success = true,
                    Data = new ProductoDTO().From(producto)
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

        // DELETE: productos/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var productoId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new ProductosManager(_serviceProvider);
                await manager.DesactivarProductoAsync(productoId);

                await RegistrarAccionAsync(productoId, nameof(Producto), "Baja");

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

        // POST: productos/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var productoId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new ProductosManager(_serviceProvider);
                await manager.ActivarProductoAsync(productoId);

                await RegistrarAccionAsync(productoId, nameof(Producto), "Alta");

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
