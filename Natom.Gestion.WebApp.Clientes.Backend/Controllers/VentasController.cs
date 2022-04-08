using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Pedidos;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Precios;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Stock;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Ventas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class VentasController : BaseController
    {
        public VentasController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        // POST: ventas/list?status={status}
        [HttpPost]
        [ActionName("list")]
        public async Task<IActionResult> PostListAsync([FromBody] DataTableRequestDTO request, [FromQuery] string status = null)
        {
            try
            {
                var manager = new VentasManager(_serviceProvider);
                var ventasCount = await manager.ObtenerVentasCountAsync();
                var ventas = await manager.ObtenerVentasDataTableAsync(request.Start, request.Length, request.Search.Value, request.Order.First().ColumnIndex, request.Order.First().Direction, status);

                return Ok(new ApiResultDTO<DataTableResponseDTO<VentaListDTO>>
                {
                    Success = true,
                    Data = new DataTableResponseDTO<VentaListDTO>
                    {
                        RecordsTotal = ventasCount,
                        RecordsFiltered = ventas.FirstOrDefault()?.CantidadFiltrados ?? 0,
                        Records = ventas.Select(venta => new VentaListDTO().From(venta)).ToList()
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

        // GET: ventas/basics/data
        // GET: ventas/basics/data?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("basics/data")]
        public async Task<IActionResult> GetBasicsDataAsync([FromQuery] string encryptedId = null)
        {
            try
            {
                var manager = new VentasManager(_serviceProvider);
                var numeroVenta = string.Empty;
                VentaDTO entity = null;

                if (!string.IsNullOrEmpty(encryptedId))
                {
                    var ventaId = EncryptionService.Decrypt<int, Venta>(Uri.UnescapeDataString(encryptedId));
                    var venta = await manager.ObtenerVentaAsync(ventaId);
                    entity = new VentaDTO().From(venta);
                    numeroVenta = entity.Numero;
                }
                else
                {
                    numeroVenta = (await manager.ObtenerSiguienteNumeroAsync()).ToString().PadLeft(8, '0');
                }

                var stockManager = new StockManager(_serviceProvider);
                var depositos = await stockManager.ObtenerDepositosActivosAsync();

                var preciosManager = new PreciosManager(_serviceProvider);
                var listasDePrecios = await preciosManager.ObtenerListasDePreciosAsync();

                var rangosHorarios = await manager.ObtenerRangosHorariosActivosAsync();

                return Ok(new ApiResultDTO<dynamic>
                {
                    Success = true,
                    Data = new
                    {
                        entity = entity,
                        depositos = depositos.Select(deposito => new DepositoDTO().From(deposito)).ToList(),
                        listasDePrecios = listasDePrecios.Select(lista => new ListaDePreciosDTO().From(lista)),
                        rangos_horarios = rangosHorarios.Select(rango => new RangoHorarioDTO().From(rango)).ToList(),
                        numero_venta = numeroVenta
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

        // GET: ventas/comprobantes/next?tipo={tipo}
        [HttpGet]
        [ActionName("comprobantes/next")]
        public async Task<IActionResult> GetComprobanteNextNumeroAsync([FromQuery] string tipo)
        {
            try
            {
                var manager = new VentasManager(_serviceProvider);
                var nextNumero = await manager.ObtenerSiguienteNumeroComprobanteAsync(tipo);

                return Ok(new ApiResultDTO<string>
                {
                    Success = true,
                    Data = nextNumero
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

        // POST: ventas/save
        [HttpPost]
        [ActionName("save")]
        public async Task<IActionResult> PostSaveAsync([FromBody] VentaDTO ventaDto)
        {
            try
            {
                var manager = new VentasManager(_serviceProvider);
                var venta = await manager.GuardarVentaAsync((int)(_accessToken?.UserId ?? 0), ventaDto);

                await RegistrarAccionAsync(venta.VentaId, nameof(Venta), "Alta");

                var ordenesDePedidoId = venta.Detalle
                                                .Where(d => d.OrdenDePedidoId.HasValue)
                                                .Select(d => d.OrdenDePedidoId.Value)
                                                .GroupBy(k => k, (k, v) => k);

                foreach (var ordenDePedidoId in ordenesDePedidoId)
                    await RegistrarAccionAsync(ordenDePedidoId, nameof(OrdenDePedido), $"Facturado en Venta N°{venta.NumeroVenta.ToString().PadLeft(8, '0')}");

                return Ok(new ApiResultDTO<VentaDTO>
                {
                    Success = true,
                    Data = new VentaDTO().From(venta)
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

        // DELETE: ventas/anular?encryptedId={encryptedId}
        [HttpPost]
        [ActionName("anular")]
        public async Task<IActionResult> AnularVentaAsync([FromQuery] string encryptedId)
        {
            try
            {
                var ordenDeVentaId = EncryptionService.Decrypt<int, Venta>(Uri.UnescapeDataString(encryptedId));

                var manager = new VentasManager(_serviceProvider);
                var pedidos = await manager.AnularVentaAsync((int)(_accessToken?.UserId ?? 0), ordenDeVentaId);

                await RegistrarAccionAsync(ordenDeVentaId, nameof(Venta), "Venta anulada");

                foreach (var pedido in pedidos)
                    await RegistrarAccionAsync(pedido.OrdenDePedidoId, nameof(OrdenDePedido), $"Venta anulada. Vuelve a pendiente de facturar.");

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

        // GET: ventas/imprimir/comprobante?encryptedId={encryptedId}
        [HttpGet]
        [ActionName("imprimir/comprobante")]
        public async Task<IActionResult> GetImprimirComprobanteAsync([FromQuery] string encryptedId)
        {
            try
            {
                var ordenDePedidoId = EncryptionService.Decrypt<int, OrdenDePedido>(encryptedId);
                var manager = new VentasManager(_serviceProvider);

                var data = manager.ObtenerDataVentaReport(ordenDePedidoId);

                string mimtype = "";
                int extension = 1;
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                var path = Path.Combine(_hostingEnvironment.ContentRootPath, "Reporting", "VentaReport.rdlc");
                var report = new LocalReport(path);
                report.AddDataSource("DataSet1", data);
                var result = report.Execute(RenderType.Pdf, extension, parameters, mimtype);
                return File(result.MainStream, "application/pdf");
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
