using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Pedidos;
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
    public class PedidosManager : BaseManager
    {
        private readonly FeatureFlagsService _featureFlagsService;

        public PedidosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _featureFlagsService = (FeatureFlagsService)serviceProvider.GetService(typeof(FeatureFlagsService));
        }

        public Task<int> ObtenerPedidosCountAsync()
                    => _db.OrdenesDePedido
                            .CountAsync();

        public async Task<List<OrdenDePedido>> ObtenerPedidosDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter, int? zonaFilter, int? transporteFilter)
        {
            var queryable = _db.OrdenesDePedido
                                    .Include(op => op.Cliente)
                                        .ThenInclude(c => c.Zona)
                                    .Include(op => op.DespachoTransporte)
                                    .Where(u => true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                int numero = 0;
                if (int.TryParse(filter, out numero))
                {
                    queryable = queryable.Where(p => p.NumeroPedido.Equals(numero)
                                                        || p.Venta.NumeroVenta.Equals(numero)
                                                        || p.NumeroRemito.Contains(numero.ToString())
                                                        || p.Venta.NumeroFactura.Contains(numero.ToString()));
                }
                else
                {
                    queryable = queryable.Where(p => p.Cliente.RazonSocial.ToLower().Contains(filter.ToLower())
                                                        || p.Cliente.Nombre.ToLower().Contains(filter.ToLower())
                                                        || p.Cliente.Apellido.ToLower().Contains(filter.ToLower())
                                                        || p.Cliente.Zona.Descripcion.ToLower().Contains(filter.ToLower())
                                                        || p.NumeroRemito.Contains(filter)
                                                        || p.DespachoTransporte.Descripcion.ToLower().Contains(filter)
                                                        || p.Venta.NumeroFactura.Contains(filter));
                }
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("PENDIENTE")) queryable = queryable.Where(q => q.Activo == true && q.PreparacionFechaHoraInicio == null && q.PreparacionFechaHoraFin == null);
                else if (statusFilter.ToUpper().Equals("EN_PREPARACION")) queryable = queryable.Where(q => q.Activo == true && q.PreparacionFechaHoraInicio != null && q.PreparacionFechaHoraFin == null);
                else if (statusFilter.ToUpper().Equals("PENDIENTE_FACTURACION")) queryable = queryable.Where(q => q.Activo == true && !q.VentaId.HasValue && q.MarcoEntregaFechaHora.HasValue);
                else if (statusFilter.ToUpper().Equals("PENDIENTE_DESPACHO")) queryable = queryable.Where(q => q.Activo == true && !q.RetiraPersonalmente && q.PreparacionFechaHoraInicio != null && q.PreparacionFechaHoraFin != null && q.DespachoFechaHora == null);
                else if (statusFilter.ToUpper().Equals("PENDIENTE_RETIRO")) queryable = queryable.Where(q => q.Activo == true && q.RetiraPersonalmente && q.PreparacionFechaHoraInicio != null && q.PreparacionFechaHoraFin != null && q.MarcoEntregaFechaHora == null);
                else if (statusFilter.ToUpper().Equals("EN_RUTA")) queryable = queryable.Where(q => q.Activo == true && !q.RetiraPersonalmente && q.PreparacionFechaHoraInicio != null && q.PreparacionFechaHoraFin != null && q.DespachoFechaHora != null && q.MarcoEntregaFechaHora == null);
                else if (statusFilter.ToUpper().Equals("ENTREGADO")) queryable = queryable = queryable.Where(q => q.Activo == true && !q.RetiraPersonalmente && q.VentaId.HasValue && q.PreparacionFechaHoraInicio != null && q.PreparacionFechaHoraFin != null && q.DespachoFechaHora != null && q.MarcoEntregaFechaHora != null);
                else if (statusFilter.ToUpper().Equals("ANULADO")) queryable = queryable.Where(q => q.Activo == false);
            }

            //FILTRO DE ZONA
            if (zonaFilter != null)
            {
                queryable = queryable.Where(q => q.Cliente.ZonaId == zonaFilter);
            }

            //FILTRO DE TRANSPORTE
            if (transporteFilter != null)
            {
                queryable = queryable.Where(q => q.DespachoTransporteId == transporteFilter);
            }

            //ORDEN
            IOrderedQueryable<OrdenDePedido> queryableOrdered;
            if (sortColumnIndex == 0)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.NumeroPedido)
                                        : queryable.OrderByDescending(c => c.NumeroPedido);
            }
            else if (sortColumnIndex == 3)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.FechaHoraPedido)
                                        : queryable.OrderByDescending(c => c.FechaHoraPedido);
            }
            else if (sortColumnIndex == 4)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.PreparacionFechaHoraFin ?? c.PreparacionFechaHoraInicio)
                                        : queryable.OrderByDescending(c => c.PreparacionFechaHoraFin ?? c.PreparacionFechaHoraInicio);
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
                                        ? queryable.OrderBy(c => sortColumnIndex == 1 ? c.NumeroRemito :
                                                                    sortColumnIndex == 2 ? c.Cliente.RazonSocial :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 1 ? c.NumeroRemito :
                                                                        sortColumnIndex == 2 ? c.Cliente.RazonSocial :
                                                            "");
            }

            var countFiltrados = queryableOrdered.Count();

            //SKIP Y TAKE
            var result = await queryableOrdered
                    .Skip(start)
                    .Take(size)
                    .ToListAsync();


            var usuariosIds = result.Where(c => c.UsuarioId.HasValue).Select(c => c.UsuarioId.Value).ToList();
            usuariosIds.AddRange(result.Where(c => c.PreparacionUsuarioId.HasValue).Select(c => c.PreparacionUsuarioId.Value).ToList());

            var usuarios = await _authService.ListUsersByIds(usuariosIds);
            result.ForEach(d => {
                d.Usuario = usuarios.FirstOrDefault(u => u.UsuarioId == d.UsuarioId);
                if (d.PreparacionUsuarioId.HasValue)
                    d.PreparacionUsuario = usuarios.FirstOrDefault(u => u.UsuarioId == d.PreparacionUsuarioId.Value);
            });


            result.ForEach(r => r.CantidadFiltrados = countFiltrados);

            return result;
        }

        public List<spReportOrdenDePedidoResult> ObtenerDataOrdenDePedidoReport(int ordenDePedidoId)
        {
             return _db.spReportOrdenDePedidoResult.FromSqlRaw("spReportOrdenDePedido {0}", ordenDePedidoId).AsEnumerable().ToList();
        }

        public List<spReportRemitoResult> ObtenerDataRemitoReport(int ordenDePedidoId)
        {
            return _db.spReportRemitoResult.FromSqlRaw("spReportRemito {0}", ordenDePedidoId).AsEnumerable().ToList();
        }

        public Task<List<OrdenDePedido>> ObtenerPedidosPendientesDeFacturacionAsync(int clienteId)
        {
            return _db.OrdenesDePedido.Where(op => op.VentaId == null && op.ClienteId == clienteId && op.Activo == true).OrderBy(op => op.NumeroPedido).ToListAsync();
        }

        public async Task<int> ObtenerSiguienteNumeroAsync()
        {
            return ((await _db.OrdenesDePedido.MaxAsync(op => (int?)op.NumeroPedido)) ?? 0) + 1;
        }

        public Task<List<RangoHorario>> ObtenerRangosHorariosActivosAsync()
        {
            return _db.RangosHorario.Where(r => r.Activo).ToListAsync();
        }

        public async Task<OrdenDePedido> ObtenerPedidoAsync(int pedidoId)
        {
            var orden = await _db.OrdenesDePedido
                                .Include(op => op.Cliente)
                                .Include(op => op.Detalle).ThenInclude(d => d.Producto)
                                .Include(op => op.Detalle).ThenInclude(d => d.Deposito)
                                .Include(op => op.Detalle).ThenInclude(d => d.ListaDePrecios)
                                .FirstAsync(op => op.OrdenDePedidoId == pedidoId);
            
            orden.Usuario = (await _authService.ListUsersByIds(new List<int>() { orden.UsuarioId.Value })).FirstOrDefault();
            
            return orden;
        }

        public async Task ValidarStockAsync(List<PedidoDetalleDTO> detallePedidoDto)
        {
            var stockManager = new StockManager(_serviceProvider);
            var stockAValidar = detallePedidoDto
                                    .Where(d => string.IsNullOrEmpty(d.EncryptedId))
                                    .GroupBy(k => new
                                    {
                                        ProductoId = EncryptionService.Decrypt<int, Producto>(k.ProductoEncryptedId),
                                        ProductoDescripcion = k.ProductoDescripcion,
                                        DepositoId = EncryptionService.Decrypt<int, Deposito>(k.DepositoEncryptedId),
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

        public async Task<OrdenDePedido> GuardarPedidoAsync(int usuarioId, PedidoDTO pedidoDto)
        {
            var ahora = DateTime.Now;
            OrdenDePedido pedido = null;
            
            if (!_featureFlagsService.FeatureFlags.Stock.PermitirVentaConStockNegativo)
                await ValidarStockAsync(pedidoDto.Detalle);

            if (_featureFlagsService.FeatureFlags.Pedidos.FechaYHoraEntregaObligatorio && pedidoDto.EntregaEstimadaFecha.HasValue && pedidoDto.EntregaEstimadaFecha.Value.Date < ahora.Date)
                throw new HandledException("La Fecha estimada de entrega debe ser mayor.");

            var productosIds = pedidoDto.Detalle.Select(d => EncryptionService.Decrypt<int, Producto>(d.ProductoEncryptedId)).ToList();
            var productos = await _db.Productos.Where(p => productosIds.Contains(p.ProductoId)).ToListAsync();

            var numeroPedido = await this.ObtenerSiguienteNumeroAsync();
            pedido = new OrdenDePedido()
            {
                NumeroPedido = numeroPedido,
                ClienteId = EncryptionService.Decrypt<int, Cliente>(pedidoDto.ClienteEncryptedId),
                FechaHoraPedido = ahora,
                RetiraPersonalmente = pedidoDto.RetiraPersonalmente,
                EntregaEstimadaFecha = pedidoDto.EntregaEstimadaFecha,
                EntregaEstimadaRangoHorarioId = EncryptionService.Decrypt<int?, RangoHorario>(pedidoDto.EntregaEstimadaRangoHorarioEncryptedId),
                EntregaDomicilio = pedidoDto.EntregaDomicilio,
                EntregaEntreCalles = pedidoDto.EntregaEntreCalles,
                EntregaLocalidad = pedidoDto.EntregaLocalidad,
                EntregaTelefono1 = pedidoDto.EntregaTelefono1,
                EntregaTelefono2 = pedidoDto.EntregaTelefono2,
                EntregaObservaciones = pedidoDto.EntregaObservaciones,
                UsuarioId = usuarioId,
                NumeroRemito = pedidoDto.NumeroRemito,
                VentaId = null,
                Activo = true,
                MedioDePago = pedidoDto.MedioDePago,
                PagoReferencia = pedidoDto.PagoReferencia,
                Observaciones = pedidoDto.Observaciones,
                PesoTotalEnGramos = pedidoDto.Detalle.Sum(d => d.ProductoPesoGramos * d.Cantidad),
                MontoTotal = pedidoDto.Detalle.Sum(d => (d.Precio * d.Cantidad) ?? 0),
                Detalle = pedidoDto.Detalle.Select(d =>
                                productos.First(p => p.ProductoId == EncryptionService.Decrypt<int, Producto>(d.ProductoEncryptedId)).MueveStock == true
                                                            ? new OrdenDePedidoDetalle
                                                                {
                                                                    ProductoId = EncryptionService.Decrypt<int, Producto>(d.ProductoEncryptedId),
                                                                    Cantidad = d.Cantidad,
                                                                    DepositoId = EncryptionService.Decrypt<int, Deposito>(d.DepositoEncryptedId),
                                                                    PesoUnitarioEnGramos = d.ProductoPesoGramos,
                                                                    ListaDePreciosId = d.PrecioListaEncryptedId.Equals("-1")
                                                                                            ? (int?)null
                                                                                            : EncryptionService.Decrypt<int, ListaDePrecios>(d.PrecioListaEncryptedId),
                                                                    Precio = d.PrecioListaEncryptedId.Equals("-1")
                                                                                            ? (decimal?)null
                                                                                            : d.Precio,
                                                                    MovimientoStock = new MovimientoStock
                                                                    {
                                                                        ProductoId = EncryptionService.Decrypt<int, Producto>(d.ProductoEncryptedId),
                                                                        FechaHora = ahora,
                                                                        UsuarioId = usuarioId,
                                                                        Tipo = "E",
                                                                        Cantidad = d.Cantidad,
                                                                        DepositoId = EncryptionService.Decrypt<int, Deposito>(d.DepositoEncryptedId),
                                                                        Observaciones = $"Pedido N°{numeroPedido.ToString().PadLeft(8, '0')}"
                                                                    }
                                                                }
                                                            : null
                                            ).ToList()
            };

            _db.OrdenesDePedido.Add(pedido);
            await _db.SaveChangesAsync();

            return pedido;
        }

        public async Task<string> ObtenerSiguienteNumeroRemitoAsync()
        {
            string siguienteNumero = null;
            var ultimoPedido = await _db.OrdenesDePedido
                                    .Where(v => v.Activo == true && v.NumeroRemito != null)
                                    .OrderByDescending(v => v.OrdenDePedidoId)
                                    .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(ultimoPedido?.NumeroRemito))
            {
                var partes = ultimoPedido.NumeroRemito.Split('-');
                long ultimaParte;
                if (long.TryParse(partes.Last(), out ultimaParte))
                {
                    ultimaParte++;
                    siguienteNumero = "";
                    for (int i = 0; i < partes.Length; i++)
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

        public async Task AnularPedidoAsync(int usuarioId, int ordenDePedidoId)
        {
            var ahora = DateTime.Now;
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;
            ordenDePedido.Activo = false;

            var movimientosStockId = await _db.OrdenesDePedidoDetalle.Where(d => d.OrdenDePedidoId == ordenDePedidoId).Select(d => d.MovimientoStockId).ToListAsync();
            var movimientosStock = await _db.MovimientosStock.Where(m => movimientosStockId.Contains(m.MovimientoStockId)).ToListAsync();

            foreach (var movimiento in movimientosStock)
            {
                _db.Entry(movimiento).State = EntityState.Modified;

                if (movimiento.ConfirmacionUsuarioId == null)
                {
                    movimiento.ConfirmacionUsuarioId = usuarioId;
                    movimiento.ConfirmacionFechaHora = ahora;
                }
                
                movimiento.Observaciones += $" (Anulado el {DateTime.Now.ToString("dd/MM/yy")})";

                var contraMovimiento = new MovimientoStock
                {
                    ProductoId = movimiento.ProductoId,
                    FechaHora = ahora,
                    UsuarioId = usuarioId,
                    Tipo = "I",
                    Cantidad = movimiento.Cantidad,
                    DepositoId = movimiento.DepositoId,
                    Observaciones = "Anulación /// " + movimiento.Observaciones,
                    ConfirmacionFechaHora = ahora,
                    ConfirmacionUsuarioId = usuarioId
                };
                _db.MovimientosStock.Add(contraMovimiento);
            }

            await _db.SaveChangesAsync();
        }

        public Task MarcarRegresoPedidoAsync(int usuarioId, int ordenDePedidoId)
        {
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;
            ordenDePedido.DespachoFechaHora = null;
            ordenDePedido.DespachoUsuarioId = null;
            return _db.SaveChangesAsync();
        }

        public async Task<bool> MarcarEntregaAsync(int usuarioId, int ordenDePedidoId, Dictionary<int, int> detalleEntrega)
        {
            var conDevoluciones = false;
            var ahora = DateTime.Now;
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;
            ordenDePedido.MarcoEntregaUsuarioId = usuarioId;
            ordenDePedido.MarcoEntregaFechaHora = ahora;

            foreach (var pedidoDetalleId in detalleEntrega.Keys)
            {
                var detalle = this._db.OrdenesDePedidoDetalle.Find(pedidoDetalleId);
                _db.Entry(detalle).State = EntityState.Modified;
                detalle.CantidadEntregada = detalleEntrega[pedidoDetalleId];

                if (detalle.CantidadEntregada != detalle.Cantidad)
                {
                    var devuelto = detalle.Cantidad - detalle.CantidadEntregada;
                    _db.MovimientosStock.Add(new MovimientoStock
                    {
                        Cantidad = devuelto.Value,
                        DepositoId = detalle.DepositoId,
                        ConfirmacionFechaHora = ahora,
                        ConfirmacionUsuarioId = usuarioId,
                        FechaHora = ahora,
                        ProductoId = detalle.ProductoId,
                        UsuarioId = usuarioId,
                        Tipo = "I",
                        Observaciones = $"Reingreso por mercadería no entregada /// Pedido N°{ordenDePedido.NumeroPedido.ToString().PadLeft(8, '0')}"
                    });
                    conDevoluciones = true;
                }
            }

            await _db.SaveChangesAsync();
            return conDevoluciones;
        }

        public async Task ModificarCantidadesAsync(int usuarioId, int ordenDePedidoId, Dictionary<int, int> detalleCantidades)
        {
            var ahora = DateTime.Now;
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;

            foreach (var pedidoDetalleId in detalleCantidades.Keys)
            {
                var detalle = this._db.OrdenesDePedidoDetalle.Find(pedidoDetalleId);
                _db.Entry(detalle).State = EntityState.Modified;

                var viejaCantidad = detalle.Cantidad;
                detalle.Cantidad = detalleCantidades[pedidoDetalleId];

                var cantidadPorVerificar = detalle.Cantidad - viejaCantidad;
                if (cantidadPorVerificar > 0)
                {
                    var stockManager = new StockManager(_serviceProvider);
                    var disponible = await stockManager.ObtenerStockActualAsync(detalle.ProductoId, detalle.DepositoId);
                    if (disponible < cantidadPorVerificar)
                        throw new HandledException($"La cantidad adicionada ({cantidadPorVerificar}) supera la disponible ({disponible})");
                }

                //SI EL PEDIDO YA ESTÁ ARMADO ENTONCES GENERAMOS UN NUEVO MOVIMIENTO DE STOCK CON MOVIMIENTOS YA CONFIRMADOS
                if (ordenDePedido.PreparacionFechaHoraFin.HasValue)
                {
                    if (viejaCantidad != detalle.Cantidad)
                    {
                        var devuelto = viejaCantidad - detalle.Cantidad;
                        if (devuelto > 0)
                        {
                            _db.MovimientosStock.Add(new MovimientoStock
                            {
                                Cantidad = devuelto,
                                DepositoId = detalle.DepositoId,
                                ConfirmacionFechaHora = ahora,
                                ConfirmacionUsuarioId = usuarioId,
                                FechaHora = ahora,
                                ProductoId = detalle.ProductoId,
                                UsuarioId = usuarioId,
                                Tipo = "I",
                                Observaciones = $"Diferencia por cambio de cantidades /// Pedido N°{ordenDePedido.NumeroPedido.ToString().PadLeft(8, '0')}"
                            });
                        }
                        else if (devuelto < 0)
                        {
                            _db.MovimientosStock.Add(new MovimientoStock
                            {
                                Cantidad = devuelto * -1,
                                DepositoId = detalle.DepositoId,
                                ConfirmacionFechaHora = ahora,
                                ConfirmacionUsuarioId = usuarioId,
                                FechaHora = ahora,
                                ProductoId = detalle.ProductoId,
                                UsuarioId = usuarioId,
                                Tipo = "E",
                                Observaciones = $"Diferencia por cambio de cantidades /// Pedido N°{ordenDePedido.NumeroPedido.ToString().PadLeft(8, '0')}"
                            });
                        }
                    }
                }
                //SI EL PEDIDO AUN NO ESTA ARMADO ENTONCES ES CUESTIÓN DE MODIFICAR LAS RESERVAS DE MOVIMIENTO DE STOCK
                else
                {
                    if (detalle.MovimientoStockId.HasValue)
                    {
                        var movimiento = this._db.MovimientosStock.Find(detalle.MovimientoStockId.Value);
                        this._db.Entry(movimiento).State = EntityState.Modified;
                        movimiento.Cantidad = detalle.Cantidad;
                    }
                }
                
            }

            await _db.SaveChangesAsync();
        }

        public Task MarcarDespachoAsync(int usuarioId, int ordenDePedidoId, int transporteId)
        {
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;
            ordenDePedido.DespachoUsuarioId = usuarioId;
            ordenDePedido.DespachoFechaHora = DateTime.Now;
            ordenDePedido.DespachoTransporteId = transporteId;
            return _db.SaveChangesAsync();
        }

        public Task MarcarCancelacionPreparacionAsync(int usuarioId, int ordenDePedidoId)
        {
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;
            ordenDePedido.PreparacionFechaHoraInicio = null;
            ordenDePedido.PreparacionUsuarioId = null;
            return _db.SaveChangesAsync();
        }

        public Task MarcarInicioPreparacionAsync(int usuarioId, int ordenDePedidoId)
        {
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;
            ordenDePedido.PreparacionFechaHoraInicio = DateTime.Now;
            ordenDePedido.PreparacionUsuarioId = usuarioId;
            return _db.SaveChangesAsync();
        }

        public async Task MarcarFinalizacionPreparacionAsync(int usuarioId, int ordenDePedidoId)
        {
            var ahora = DateTime.Now;
            var ordenDePedido = this._db.OrdenesDePedido.Find(ordenDePedidoId);
            _db.Entry(ordenDePedido).State = EntityState.Modified;
            ordenDePedido.PreparacionFechaHoraFin = ahora;

            var movimientosStockId = await _db.OrdenesDePedidoDetalle.Where(d => d.OrdenDePedidoId == ordenDePedidoId).Select(d => d.MovimientoStockId).ToListAsync();
            var movimientosStock = await _db.MovimientosStock.Where(m => movimientosStockId.Contains(m.MovimientoStockId)).ToListAsync();

            foreach (var movimiento in movimientosStock)
            {
                _db.Entry(movimiento).State = EntityState.Modified;
                movimiento.ConfirmacionUsuarioId = usuarioId;
                movimiento.ConfirmacionFechaHora = ahora;
            }

            await _db.SaveChangesAsync();
        }
    }
}
