using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Stock;
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
    public class StockManager : BaseManager
    {
        public StockManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public List<spMovimientosStockListResult> ObtenerMovimientosStockDataTable(int start, int size, string filter, int? depositoId, int? productoId, DateTime? fecha)
        {
            return _db.spMovimientosStockListResult
                                .FromSqlRaw("spMovimientosStockList {0}, {1}, {2}, {3}, {4}, {5}", depositoId, productoId, filter, start, size, fecha)
                                .AsEnumerable()
                                .ToList();
        }

        public Task<List<Deposito>> ObtenerDepositosActivosAsync()
        {
            return _db.Depositos.Where(d => d.Activo).ToListAsync();
        }

        public async Task GuardarMovimientoAsync(int usuarioId, MovimientoStockDTO movimientoDto)
        {
            var productoId = EncryptionService.Decrypt<int>(movimientoDto.ProductoEncryptedId);
            var depositoId = EncryptionService.Decrypt<int>(movimientoDto.DepositoEncryptedId);
            
            if (movimientoDto.Tipo == "E")
            {
                var cantidadActual = await ObtenerStockActualAsync(productoId, depositoId);
                if (cantidadActual - movimientoDto.Cantidad < 0)
                    throw new HandledException($"No es posible realizar el Egreso de Mercadería indicado ya que la cantidad ingresada ({movimientoDto.Cantidad}) es superior al disponible actual ({cantidadActual})");
            }

            var movimiento = new MovimientoStock
            {
                ProductoId = productoId,
                DepositoId = depositoId,
                FechaHora = DateTime.Now,
                Cantidad = movimientoDto.Cantidad,
                Tipo = movimientoDto.Tipo,
                Observaciones = movimientoDto.Observaciones,
                UsuarioId = usuarioId,
                ConfirmacionFechaHora = DateTime.Now,
                ConfirmacionUsuarioId = usuarioId,
                EsCompra = movimientoDto.EsCompra
            };

            //if (movimientoDto.EsCompra)
            //{
            //    movimiento.ProveedorId = EncryptionService.Decrypt<int>(movimientoDto.ProveedorEncryptedId);
            //    movimiento.CostoUnitario = movimientoDto.CostoUnitario;

            //    var producto = await _db.Productos.FindAsync(productoId);
            //    _db.Entry(producto).State = EntityState.Modified;
            //    producto.ProveedorId = movimiento.ProveedorId;
            //    producto.CostoUnitario = (movimiento.CostoUnitario ?? producto.CostoUnitario);
            //}

            _db.MovimientosStock.Add(movimiento);
            await _db.SaveChangesAsync();
        }

        public Task<int> ObtenerStockActualAsync(int productoId, int? depositoId)
        {
            return _db.MovimientosStock
                        .Where(m => m.ProductoId == productoId && m.DepositoId == (depositoId ?? m.DepositoId))
                        .SumAsync(m => (int?)(m.Tipo == "I" ? m.Cantidad : m.Cantidad * -1) ?? 0);
        }

        public async Task MarcarMovimientosComoControladoAsync(int usuarioId, int movimientoStockId)
        {
            var ahora = DateTime.Now;
            var ultimoMovimiento = await _db.MovimientosStock.FirstAsync(m => m.MovimientoStockId == movimientoStockId);
            var movimientosAnteriores = await _db.MovimientosStock.Where(m => m.MovimientoStockId <= ultimoMovimiento.MovimientoStockId
                                                                            && m.DepositoId == ultimoMovimiento.DepositoId
                                                                            && m.ProductoId == ultimoMovimiento.ProductoId
                                                                            && m.FechaHoraControlado == null).ToListAsync();
            foreach (var movimiento in movimientosAnteriores)
            {
                _db.Entry(movimiento).State = EntityState.Modified;
                movimiento.FechaHoraControlado = ahora;
                movimiento.ControladoUsuarioId = usuarioId;
            }
            await _db.SaveChangesAsync();
        }
    }
}
