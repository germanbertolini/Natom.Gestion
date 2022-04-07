using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Productos;
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
    public class CategoriasProductosController : BaseController
    {
        public CategoriasProductosController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: categoriasproductos/list?filter={filter}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new CategoriasProductosManager(_serviceProvider);
                var usuariosCount = await manager.ObtenerCategoriasProductosCountAsync();
                var usuarios = await manager.ObtenerCategoriasProductosDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, statusFilter: status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<CategoriaProductoDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<CategoriaProductoDTO>
                    {
                        RecordsTotal = usuariosCount,
                        RecordsFiltered = usuarios.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = usuarios.Select(usuario => new CategoriaProductoDTO().From(usuario)).ToList()
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

        // GET: categoriasproductos/basics/data
        // GET: categoriasproductos/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new CategoriasProductosManager(_serviceProvider);
                CategoriaProductoDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var categoriaProductoId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));
                    var categoriaProducto = await manager.ObtenerCategoriaProductoAsync(categoriaProductoId);
                    entity = new CategoriaProductoDTO().From(categoriaProducto);
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

        // POST: categoriasproductos/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] CategoriaProductoDTO categoriaProductoDto)
        {
            try
            {
                var manager = new CategoriasProductosManager(_serviceProvider);
                var categoriaProducto = await manager.GuardarCategoriaProductoAsync(categoriaProductoDto);

                await RegistrarAccionAsync(categoriaProducto.CategoriaProductoId, nameof(CategoriaProducto), string.IsNullOrEmpty(categoriaProductoDto.EncryptedId) ? "Alta" : "Edición");

                return Ok(new ApiResultDTO<CategoriaProductoDTO>
                {
                    Success = true,
                    Data = new CategoriaProductoDTO().From(categoriaProducto)
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

        // DELETE: categoriasproductos/disable?encryptedId={encryptedId}
        [HttpDelete]
        [ActionName("disable")]
        public async Task<IActionResult> DeleteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var categoriaProductoId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new CategoriasProductosManager(_serviceProvider);
                await manager.DesactivarCategoriaProductoAsync(categoriaProductoId);

                await RegistrarAccionAsync(categoriaProductoId, nameof(CategoriaProducto), "Baja");

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

        // POST: categoriasproductos/enable?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("enable")]
        public async Task<IActionResult> EnableAsync([FromQuery] string encryptedId)
        {
            try
            {
                var categoriaProductoId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(encryptedId));

                var manager = new CategoriasProductosManager(_serviceProvider);
                await manager.ActivarCategoriaProductoAsync(categoriaProductoId);

                await RegistrarAccionAsync(categoriaProductoId, nameof(CategoriaProducto), "Alta");

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
