using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Stock;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class StockController : BaseController
    {
        public StockController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: stock/list?depositoFilter={depositoFilter}&productoFilter={productoFilter}&filtroFecha={DD/MM/YYYY}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string depositoFilter = null, [FromQuery] string productoFilter = null, [FromQuery] string filtroFecha = null)
        {
            try
            {
                var manager = new StockManager(_serviceProvider);
                int? depositoId = null;
                int? productoId = null;
                DateTime? fecha = null;

                if (!string.IsNullOrEmpty(depositoFilter)) depositoId = EncryptionService.Decrypt<int, Deposito>(depositoFilter);
                if (!string.IsNullOrEmpty(productoFilter)) productoId = EncryptionService.Decrypt<int, Producto>(productoFilter);
                if (!string.IsNullOrEmpty(filtroFecha)) fecha = Convert.ToDateTime(filtroFecha);

                var movimientos = manager.ObtenerMovimientosStockDataTable(request.Start, request.Length, request.Search.Value, depositoId, productoId, fecha);
                var depositos = await manager.ObtenerDepositosActivosAsync();

                return Ok(new ApiResultDTO<DataTableResponseDTO<StockListDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<StockListDTO>
                    {
                        RecordsTotal = movimientos.FirstOrDefault()?.CantidadRegistros ?? 0,
                        RecordsFiltered = movimientos.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = movimientos.Select(movimiento => new StockListDTO().From(movimiento)).ToList(),
                        ExtraData = new
                        {
                            depositos = depositos.Select(deposito => new DepositoDTO().From(deposito))
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
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: stock/basics/data
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync()
        {
            try
            {
                var manager = new StockManager(_serviceProvider);
                var depositos = await manager.ObtenerDepositosActivosAsync();

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        depositos = depositos.Select(deposito => new DepositoDTO().From(deposito)).ToList()
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

        // GET: stock/quantity?producto={encryptedProductoId}&deposito={encryptedDepositoId}
        [HttpGet]
        [ActionName("quantity")]
        public async Task<IActionResult> GetQuantityAsync([FromQuery]string producto, [FromQuery]string deposito = null)
        {
            try
            {
                int productoId = EncryptionService.Decrypt<int, Producto>(producto);
                int? depositoId = null;

                if (!string.IsNullOrEmpty(deposito))
                    depositoId = EncryptionService.Decrypt<int, Deposito>(deposito);


                var manager = new StockManager(_serviceProvider);
                var stockActual = await manager.ObtenerStockActualAsync(productoId, depositoId);

                return Ok(new ApiResultDTO<int>
                {
                    Success = true,
                    Data = stockActual
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

        // POST: stock/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] MovimientoStockDTO movimientoDto)
        {
            try
            {
                var manager = new StockManager(_serviceProvider);
                await manager.GuardarMovimientoAsync((int)(_accessToken?.UserId ?? 0), movimientoDto);

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


        // POST: stock/control?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("control")]
        public async Task<IActionResult> PostSaveAsync([FromQuery] string encryptedId)
        {
            try
            {
                int movimientoStockId = EncryptionService.Decrypt<int, MovimientoStock>(encryptedId);

                var manager = new StockManager(_serviceProvider);
                await manager.MarcarMovimientosComoControladoAsync((int)(_accessToken?.UserId ?? 0), movimientoStockId);

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
