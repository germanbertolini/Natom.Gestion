using Microsoft.EntityFrameworkCore;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Ventas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class VentasManager : BaseManager
    {
        private readonly FeatureFlagsService _featureFlagsService;

        public VentasManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _featureFlagsService = (FeatureFlagsService)serviceProvider.GetService(typeof(FeatureFlagsService));
        }

        public Task<int> ObtenerVentasCountAsync()
                    => _db.Ventas
                            .CountAsync();

        public async Task<List<Venta>> ObtenerVentasDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter = null)
        {
            var queryable = _db.Ventas
                                    .Include(op => op.Cliente)
                                    .Include(op => op.Usuario)
                                    .Include(op => op.Detalle)
                                            .ThenInclude(d => d.OrdenDePedido)
                                    .Where(u => true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                int numero = 0;
                if (int.TryParse(filter, out numero))
                {
                    queryable = queryable.Where(p => p.NumeroVenta.Equals(numero)
                                                        || p.NumeroFactura.Contains(numero.ToString())
                                                        || p.Detalle.Any(d => d.OrdenDePedido.NumeroPedido == numero)
                                                        || p.Detalle.Any(d => d.OrdenDePedido.NumeroRemito.Contains(numero.ToString())));
                }
                else
                {
                    queryable = queryable.Where(p => p.Cliente.RazonSocial.ToLower().Contains(filter.ToLower())
                                                        || p.Cliente.Nombre.ToLower().Contains(filter.ToLower())
                                                        || p.Cliente.Apellido.ToLower().Contains(filter.ToLower())
                                                        || p.NumeroFactura.Contains(filter)
                                                        || p.Detalle.Any(d => d.OrdenDePedido.NumeroRemito.Contains(filter)));
                }
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("FACTURADO")) queryable = queryable.Where(q => q.Activo == true);
                else if (statusFilter.ToUpper().Equals("ANULADO")) queryable = queryable.Where(q => q.Activo == false);
            }

            //ORDEN
            IOrderedQueryable<Venta> queryableOrdered;
            if (sortColumnIndex == 0)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.NumeroVenta)
                                        : queryable.OrderByDescending(c => c.NumeroVenta);
            }
            else if (sortColumnIndex == 3)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.FechaHoraVenta)
                                        : queryable.OrderByDescending(c => c.FechaHoraVenta);
            }
            else if (sortColumnIndex == 6)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.PesoTotalEnGramos)
                                        : queryable.OrderByDescending(c => c.PesoTotalEnGramos);
            }
            else
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 1 ? c.TipoFactura + " " + c.NumeroFactura :
                                                                    sortColumnIndex == 2 ? c.Cliente.RazonSocial :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 1 ? c.TipoFactura + " " + c.NumeroFactura :
                                                                        sortColumnIndex == 2 ? c.Cliente.RazonSocial :
                                                            "");
            }

            var countFiltrados = queryableOrdered.Count();

            //SKIP Y TAKE
            var result = await queryableOrdered
                    .Skip(start)
                    .Take(size)
                    .ToListAsync();

            result.ForEach(r => r.CantidadFiltrados = countFiltrados);

            return result;
        }

        public async Task<int> ObtenerSiguienteNumeroAsync()
        {
            return ((await _db.Ventas.MaxAsync(op => (int?)op.NumeroVenta)) ?? 0) + 1;
        }

        public Task<List<RangoHorario>> ObtenerRangosHorariosActivosAsync()
        {
            return _db.RangosHorario.Where(r => r.Activo).ToListAsync();
        }

        public Task<Venta> ObtenerVentaAsync(int ventaId)
        {
            return _db.Ventas
                        .Include(op => op.Cliente)
                        .Include(op => op.Usuario)
                        .Include(op => op.Detalle).ThenInclude(d => d.Producto)
                        .Include(op => op.Detalle).ThenInclude(d => d.Deposito)
                        .Include(op => op.Detalle).ThenInclude(d => d.ListaDePrecios)
                        .Include(op => op.Detalle).ThenInclude(d => d.OrdenDePedidoDetalle).ThenInclude(d => d.OrdenDePedido)
                        .FirstAsync(op => op.VentaId == ventaId);
        }

        public async Task<string> ObtenerSiguienteNumeroComprobanteAsync(string tipo)
        {
            string siguienteNumero = null;
            var ultimaVenta = await _db.Ventas
                                    .Where(v => v.Activo && v.TipoFactura.ToUpper().Equals(tipo.ToUpper()) && v.NumeroFactura != null)
                                    .OrderByDescending(v => v.VentaId)
                                    .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(ultimaVenta?.NumeroFactura))
            {
                var partes = ultimaVenta.NumeroFactura.Split('-');
                long ultimaParte;
                if (long.TryParse(partes.Last(), out ultimaParte))
                {
                    ultimaParte++;
                    siguienteNumero = "";
                    for (int i = 0; i < partes.Length; i ++)
                    {
                        if (i + 1 != partes.Length) //SI NO ES LA ULTIMA
                            siguienteNumero += partes[i] + "-";
                        else
                            siguienteNumero += ultimaParte.ToString().PadLeft(8, '0');
                    }
                }
            }

            return siguienteNumero;
        }

        public async Task ValidarStockAsync(List<VentaDetalleDTO> detallePedidoDto)
        {
            var stockManager = new StockManager(_serviceProvider);
            var stockAValidar = detallePedidoDto
                                    .Where(d => string.IsNullOrEmpty(d.EncryptedId))
                                    .GroupBy(k => new
                                    {
                                        ProductoId = EncryptionService.Decrypt<int>(k.ProductoEncryptedId),
                                        ProductoDescripcion = k.ProductoDescripcion,
                                        DepositoId = EncryptionService.Decrypt<int>(k.DepositoEncryptedId),
                                        DepositoDescripcion = k.DepositoDescripcion
                                    },
                                             (k, v) => new
                                             {
                                                 ProductoId = k.ProductoId,
                                                 ProductoDescripcion = k.ProductoDescripcion,
                                                 DepositoId = k.DepositoId,
                                                 DepositoDescripcion = k.DepositoDescripcion,
                                                 Cantidad = v.Sum(p => p.Cantidad)
                                             })
                                    .ToList();
            foreach (var item in stockAValidar)
            {
                int cantidad = await stockManager.ObtenerStockActualAsync(item.ProductoId, item.DepositoId);
                if (cantidad < item.Cantidad)
                    throw new HandledException($"No hay stock disponible para '{item.ProductoDescripcion}' en '{item.DepositoDescripcion}'. Cantidad pedido: {item.Cantidad} / Cantidad disponible actual: {cantidad}");
            }
        }

        public async Task<Venta> GuardarVentaAsync(int usuarioId, VentaDTO ventaDto)
        {
            var ahora = DateTime.Now;
            Venta venta = null;

            if (!_featureFlagsService.FeatureFlags.Stock.PermitirVentaConStockNegativo)
                await ValidarStockAsync(ventaDto.Detalle);

            if (_db.Ventas.Any(v => v.Activo && v.TipoFactura == ventaDto.TipoFactura && v.NumeroFactura == ventaDto.NumeroFactura))
                throw new HandledException("Ya existe una Facturación con mismo comprobante.");

            var numeroVenta = await this.ObtenerSiguienteNumeroAsync();
            venta = new Venta()
            {
                NumeroVenta = numeroVenta,
                ClienteId = EncryptionService.Decrypt<int>(ventaDto.ClienteEncryptedId),
                FechaHoraVenta = ahora,
                UsuarioId = usuarioId,
                NumeroFactura = ventaDto.NumeroFactura,
                TipoFactura = ventaDto.TipoFactura,
                Activo = true,
                Observaciones = ventaDto.Observaciones,
                MedioDePago = ventaDto.MedioDePago,
                PagoReferencia = ventaDto.PagoReferencia,
                PesoTotalEnGramos = ventaDto.Detalle.Sum(d => (d.ProductoPesoGramos ?? 0) * d.Cantidad),
                MontoTotal = ventaDto.Detalle.Sum(d => (d.Precio * d.Cantidad) ?? 0),
                Detalle = ventaDto.Pedidos.Select(d => new VentaDetalle
                                                        {
                                                            ProductoId = EncryptionService.Decrypt<int>(d.ProductoEncryptedId),
                                                            Cantidad = d.Cantidad,
                                                            DepositoId = EncryptionService.Decrypt<int>(d.DepositoEncryptedId),
                                                            PesoUnitarioEnGramos = d.ProductoPesoGramos,
                                                            NumeroRemito = d.NumeroRemito,
                                                            OrdenDePedidoId = EncryptionService.Decrypt<int>(d.OrdenDePedidoEncryptedId),
                                                            OrdenDePedidoDetalleId = EncryptionService.Decrypt<int>(d.OrdenDePedidoDetalleEncryptedId),
                                                            ListaDePreciosId = (d.PrecioListaEncryptedId?.Equals("-1") ?? true)
                                                                                        ? (int?)null
                                                                                        : EncryptionService.Decrypt<int>(d.PrecioListaEncryptedId),
                                                            Precio = (decimal)d.Precio
                                                        }
                                            ).ToList()
            };

            //AGREGAMOS AL DETALLE LOS ITEMS AGREGADOS SIN PEDIDO
            if (ventaDto.Detalle != null)
            {
                foreach (var d in ventaDto.Detalle)
                {
                    venta.Detalle.Add(new VentaDetalle
                    {
                        ProductoId = EncryptionService.Decrypt<int>(d.ProductoEncryptedId),
                        Cantidad = d.Cantidad,
                        DepositoId = EncryptionService.Decrypt<int>(d.DepositoEncryptedId),
                        PesoUnitarioEnGramos = d.ProductoPesoGramos,
                        ListaDePreciosId = EncryptionService.Decrypt<int>(d.PrecioListaEncryptedId),
                        Precio = (decimal)d.Precio
                    });
                }
            }

            venta.ComposicionPagoCajaDiaria = new List<MovimientoCajaDiaria>();
            venta.ComposicionPagoCajaFuerte = new List<MovimientoCajaFuerte>();
            venta.ComposicionPagoCuentaCorriente = new List<MovimientoCtaCteCliente>();

            //REALIZAMOS EL PAGO
            switch (venta.MedioDePago)
            {
                case "Efectivo":
                    RealizarPagoEnEfectivo(venta);
                    break;
                case "Cheque":
                    RealizarPagoConCheque(venta);
                    break;
                case "Mercado Pago":
                    RealizarPagoConMercadoPago(venta);
                    break;
                case "Tarjeta":
                    RealizarPagoConTarjeta(venta);
                    break;
                case "Transferencia":
                    RealizarPagoConTransferencia(venta);
                    break;
                case "Cuenta Corriente":
                    RealizarPagoConCuentaCorriente(venta);
                    break;
            }

            _db.Ventas.Add(venta);

            await _db.SaveChangesAsync();


            ///MOVEMOS EL STOCK DE LOS PRODUCTOS QUE SALIERON SIN ORDEN DE PEDIDO
            if (ventaDto.Detalle != null)
            {
                foreach (var d in ventaDto.Detalle)
                {
                    _db.MovimientosStock.Add(new MovimientoStock
                    {
                        ProductoId = EncryptionService.Decrypt<int>(d.ProductoEncryptedId),
                        FechaHora = ahora,
                        UsuarioId = usuarioId,
                        Tipo = "E",
                        Cantidad = d.Cantidad,
                        ConfirmacionFechaHora = ahora,
                        ConfirmacionUsuarioId = usuarioId,
                        DepositoId = EncryptionService.Decrypt<int>(d.DepositoEncryptedId),
                        Observaciones = $"Venta N°{venta.NumeroVenta.ToString().PadLeft(8, '0')}"
                    });
                }
            }


            ///VINCULAMOS LAS ORDENES DE PEDIDO A LA VENTA
            var ordenesDePedidoId = ventaDto.Pedidos
                                                .Where(d => !string.IsNullOrEmpty(d.OrdenDePedidoEncryptedId) && d.OrdenDePedidoEncryptedId != "-1")
                                                .Select(d => EncryptionService.Decrypt<int>(d.OrdenDePedidoEncryptedId))
                                                .ToList();
            var ordenesDePedido = await _db.OrdenesDePedido
                                                .Include(op => op.Detalle).ThenInclude(d => d.MovimientoStock)
                                                .Where(op => ordenesDePedidoId.Contains(op.OrdenDePedidoId))
                                                .ToListAsync();
            
            foreach (var pedido in ordenesDePedido)
            {
                _db.Entry(pedido).State = EntityState.Modified;
                pedido.VentaId = venta.VentaId;

                foreach (var detalle in pedido.Detalle)
                {
                    _db.Entry(detalle.MovimientoStock).State = EntityState.Modified;
                    detalle.MovimientoStock.Observaciones = detalle.MovimientoStock.Observaciones + " /// " + $"Venta N°{venta.NumeroVenta.ToString().PadLeft(8, '0')}";
                }
            }

            await _db.SaveChangesAsync();


            return venta;
        }

        private void RealizarPagoConTransferencia(Venta venta)
        {
            venta.ComposicionPagoCajaFuerte.Add(new MovimientoCajaFuerte
            {
                FechaHora = venta.FechaHoraVenta,
                Importe = venta.MontoTotal,
                MedioDePago = "Transferencia",
                Tipo = "C",
                UsuarioId = venta.UsuarioId,
                Referencia = venta.PagoReferencia,
                Observaciones = $"PAGO CON TRANSFERENCIA {(string.IsNullOrEmpty(venta.PagoReferencia) ? "" : $" /// REFERENCIA {venta.PagoReferencia.ToUpper()}")} /// VENTA n°{venta.NumeroVenta.ToString().PadLeft(8, '0')}"
            });
        }

        private void RealizarPagoConCuentaCorriente(Venta venta)
        {
            var cliente = _db.Clientes.Find(venta.ClienteId);
            if (cliente.MontoCtaCte == 0)
                throw new HandledException("El cliente no posee Cuenta Corriente.");

            var ctaCteManager = new CuentasCorrientesManager(_serviceProvider);
            var saldoDisponible = ctaCteManager.ObtenerDisponibleActualCtaCteClienteAsync(cliente.ClienteId).GetAwaiter().GetResult();
            if (venta.MontoTotal > saldoDisponible)
                throw new HandledException($"Fondos de la Cuenta Corriente insuficientes ({saldoDisponible.ToString("C2")}). Monto de operación actual: {venta.MontoTotal.ToString("C2")}");

            venta.ComposicionPagoCuentaCorriente.Add(new MovimientoCtaCteCliente
            {
                FechaHora = venta.FechaHoraVenta,
                Importe = venta.MontoTotal,
                Tipo = "D",
                UsuarioId = venta.UsuarioId,
                ClienteId = venta.ClienteId,
                Observaciones = $"VENTA POR CUENTA CORRIENTE {(string.IsNullOrEmpty(venta.PagoReferencia) ? "" : $" /// REFERENCIA {venta.PagoReferencia.ToUpper()}")} /// VENTA n°{venta.NumeroVenta.ToString().PadLeft(8, '0')}"
            });
        }

        private void RealizarPagoConTarjeta(Venta venta)
        {
            venta.ComposicionPagoCajaFuerte.Add(new MovimientoCajaFuerte
            {
                FechaHora = venta.FechaHoraVenta,
                Importe = venta.MontoTotal,
                MedioDePago = "Tarjeta",
                Tipo = "C",
                UsuarioId = venta.UsuarioId,
                Referencia = venta.PagoReferencia,
                Observaciones = $"PAGO CON TARJETA {(string.IsNullOrEmpty(venta.PagoReferencia) ? "" : $" /// REFERENCIA {venta.PagoReferencia.ToUpper()}")} /// VENTA n°{venta.NumeroVenta.ToString().PadLeft(8, '0')}"
            });
        }

        private void RealizarPagoConMercadoPago(Venta venta)
        {
            venta.ComposicionPagoCajaFuerte.Add(new MovimientoCajaFuerte
            {
                FechaHora = venta.FechaHoraVenta,
                Importe = venta.MontoTotal,
                MedioDePago = "Mercado Pago",
                Tipo = "C",
                UsuarioId = venta.UsuarioId,
                Referencia = venta.PagoReferencia,
                Observaciones = $"PAGO CON MERCADO PAGO {(string.IsNullOrEmpty(venta.PagoReferencia) ? "" : $" /// REFERENCIA {venta.PagoReferencia.ToUpper()}")} /// VENTA n°{venta.NumeroVenta.ToString().PadLeft(8, '0')}"
            });
        }

        private void RealizarPagoConCheque(Venta venta)
        {
            venta.ComposicionPagoCajaFuerte.Add(new MovimientoCajaFuerte
            {
                FechaHora = venta.FechaHoraVenta,
                Importe = venta.MontoTotal,
                MedioDePago = "Cheque",
                EsCheque = true,
                Tipo = "C",
                UsuarioId = venta.UsuarioId,
                Referencia = venta.PagoReferencia,
                Observaciones = $"PAGO CON CHEQUE {(string.IsNullOrEmpty(venta.PagoReferencia) ? "" : $" /// REFERENCIA {venta.PagoReferencia.ToUpper()}")} /// VENTA n°{venta.NumeroVenta.ToString().PadLeft(8, '0')}"
            });
        }

        private void RealizarPagoEnEfectivo(Venta venta)
        {
            venta.ComposicionPagoCajaDiaria.Add(new MovimientoCajaDiaria
            {
                FechaHora = venta.FechaHoraVenta,
                Importe = venta.MontoTotal,
                MedioDePago = "Efectivo",
                Tipo = "C",
                UsuarioId = venta.UsuarioId,
                Referencia = venta.PagoReferencia,
                Observaciones = $"PAGO CON EFECTIVO {(string.IsNullOrEmpty(venta.PagoReferencia) ? "" : $" /// REFERENCIA {venta.PagoReferencia.ToUpper()}" )} /// VENTA n°{venta.NumeroVenta.ToString().PadLeft(8, '0')}"
            });
        }

        public List<spReportVentaResult> ObtenerDataVentaReport(int ventaId)
        {
            return _db.spReportVentaResult.FromSqlRaw("spReportVenta {0}", ventaId).AsEnumerable().ToList();
        }

        public async Task<List<OrdenDePedido>> AnularVentaAsync(int usuarioId, int ventaId)
        {
            var ahora = DateTime.Now;
            var venta = this._db.Ventas
                                    .Include(d => d.Detalle)
                                    .Include(d => d.ComposicionPagoCajaDiaria)
                                    .Include(d => d.ComposicionPagoCajaFuerte)
                                    .First(v => v.VentaId == ventaId);
            _db.Entry(venta).State = EntityState.Modified;
            venta.Activo = false;

            ///DESVINCULAMOS LAS ORDENES DE PEDIDO DE LA VENTA
            var pedidos = await _db.OrdenesDePedido.Where(op => op.VentaId == ventaId).ToListAsync();
            foreach (var pedido in pedidos)
            {
                _db.Entry(pedido).State = EntityState.Modified;
                pedido.VentaId = null;
            }

            ///REINGRESAMOS EL STOCK DE LOS PRODUCTOS SIN ORDEN DE PEDIDO
            foreach (var detalle in venta.Detalle.Where(d => !d.OrdenDePedidoDetalleId.HasValue))
            {
                var contraMovimiento = new MovimientoStock
                {
                    ProductoId = detalle.ProductoId,
                    FechaHora = ahora,
                    UsuarioId = usuarioId,
                    Tipo = "I",
                    Cantidad = detalle.Cantidad,
                    DepositoId = detalle.DepositoId,
                    Observaciones = "Anulación /// " + $"Venta N°{venta.NumeroVenta.ToString().PadLeft(8, '0')}",
                    ConfirmacionFechaHora = ahora,
                    ConfirmacionUsuarioId = usuarioId
                };
                _db.MovimientosStock.Add(contraMovimiento);
            }

            //REVERSAMOS PAGOS EN CAJA DIARIA
            var pagosDiaria = new List<MovimientoCajaDiaria>();
            venta.ComposicionPagoCajaDiaria.ForEach(c => pagosDiaria.Add(c));
            foreach (var diaria in pagosDiaria)
            {
                var contraMovimiento = new MovimientoCajaDiaria
                {
                    FechaHora = ahora,
                    UsuarioId = usuarioId,
                    Tipo = "D",
                    Importe = diaria.Importe,
                    Observaciones = "REVERSO POR ANULACIÓN DE VENTA /// " + diaria.Observaciones,
                    EsCheque = diaria.EsCheque,
                    VentaId = diaria.VentaId,
                    MedioDePago = diaria.MedioDePago,
                    Referencia = diaria.Referencia
                };
                _db.MovimientosCajaDiaria.Add(contraMovimiento);
            }

            //REVERSAMOS PAGOS EN CAJA FUERTE
            var pagosFuerte = new List<MovimientoCajaFuerte>();
            venta.ComposicionPagoCajaFuerte.ForEach(c => pagosFuerte.Add(c));
            foreach (var diaria in pagosFuerte)
            {
                var contraMovimiento = new MovimientoCajaFuerte
                {
                    FechaHora = ahora,
                    UsuarioId = usuarioId,
                    Tipo = "D",
                    Importe = diaria.Importe,
                    Observaciones = "REVERSO POR ANULACIÓN DE VENTA /// " + diaria.Observaciones,
                    EsCheque = diaria.EsCheque,
                    VentaId = diaria.VentaId,
                    MedioDePago = diaria.MedioDePago,
                    Referencia = diaria.Referencia
                };
                _db.MovimientosCajaFuerte.Add(contraMovimiento);
            }


            await _db.SaveChangesAsync();

            return pedidos;
        }
    }
}
