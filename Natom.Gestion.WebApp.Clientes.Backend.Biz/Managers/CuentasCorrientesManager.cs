using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Clientes.CtaCte;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Proveedores.CtaCte;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class CuentasCorrientesManager : BaseManager
    {
        public CuentasCorrientesManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerMovimientosCtaCteClienteCountAsync(int clienteId)
                    => _db.MovimientosCtaCteCliente
                            .Where(m => m.ClienteId == clienteId)
                            .CountAsync();

        public async Task<List<MovimientoCtaCteCliente>> ObtenerMovimientosCtaCteClienteDataTableAsync(int clienteId, int start, int size, string filter, int sortColumnIndex, string sortDirection, DateTime? dateTimeFilter)
        {
            var queryable = _db.MovimientosCtaCteCliente.Include(m => m.Usuario).Where(u => u.ClienteId == clienteId);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Observaciones.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO FECHA
            if (dateTimeFilter != null)
            {
                queryable = queryable.Where(q => q.FechaHora.Date.Equals(dateTimeFilter));
            }

            //ORDEN
            IOrderedQueryable<MovimientoCtaCteCliente> queryableOrdered;
            if (sortColumnIndex == 0)   //FECHA HORA
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.FechaHora.Date).ThenBy(c => c.FechaHora.TimeOfDay)
                                        : queryable.OrderByDescending(c => c.FechaHora.Date).ThenByDescending(c => c.FechaHora.TimeOfDay);
            }
            else if (sortColumnIndex == 3) //IMPORTE (DECIMAL)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.Importe)
                                        : queryable.OrderByDescending(c => c.Importe);
            }
            else //OTROS (STRING)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 1 ? c.Usuario.Nombre :
                                                                    sortColumnIndex == 2 ? c.Tipo :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 1 ? c.Usuario.Nombre :
                                                                    sortColumnIndex == 2 ? c.Tipo :
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

        public async Task<decimal> ObtenerDisponibleActualCtaCteClienteAsync(int clienteId)
        {
            var monto = (await _db.Clientes.Where(c => c.ClienteId == clienteId).FirstOrDefaultAsync())?.MontoCtaCte ?? 0;

            var queryable = _db.MovimientosCtaCteCliente.Where(u => u.ClienteId == clienteId);

            var saldo = await queryable.SumAsync(q => (decimal?)(q.Tipo.Equals("C") ? q.Importe : q.Importe * -1) ?? 0);

            return monto + saldo;
        }

        public async Task<MovimientoCtaCteCliente> GuardarMovimientoCtaCteClienteAsync(ClienteCtaCteMovimientoDTO movimientoDto, int usuarioId)
        {
            int clienteId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(movimientoDto.EncryptedClienteId));

            if (movimientoDto.Tipo == "D")
            {
                var saldoActual = await ObtenerDisponibleActualCtaCteClienteAsync(clienteId);
                if (saldoActual - movimientoDto.Importe < 0)
                    throw new HandledException($"No es posible realizar el Egreso ya que el importe ingresado ({movimientoDto.Importe.ToString("C2")}) es superior al disponible actual de la cuenta ({saldoActual.ToString("C2")})");
            }

            var movimiento = new MovimientoCtaCteCliente()
            {
                FechaHora = DateTime.Now,
                Importe = movimientoDto.Importe,
                Observaciones = movimientoDto.Observaciones,
                Tipo = movimientoDto.Tipo,
                UsuarioId = usuarioId,
                ClienteId = clienteId
            };

            _db.MovimientosCtaCteCliente.Add(movimiento);
            await _db.SaveChangesAsync();

            return movimiento;
        }

        public async Task<decimal> ObtenerMontoActualCtaCteClienteAsync(int clienteId)
        {
            return (await _db.Clientes.Where(c => c.ClienteId == clienteId).FirstOrDefaultAsync())?.MontoCtaCte ?? 0;
        }


        public Task<int> ObtenerMovimientosCtaCteProveedorCountAsync(int proveedorId)
                    => _db.MovimientosCtaCteProveedor
                            .Where(m => m.ProveedorId == proveedorId)
                            .CountAsync();

        public async Task<List<MovimientoCtaCteProveedor>> ObtenerMovimientosCtaCteProveedorDataTableAsync(int proveedorId, int start, int size, string filter, int sortColumnIndex, string sortDirection, DateTime? dateTimeFilter)
        {
            var queryable = _db.MovimientosCtaCteProveedor.Include(m => m.Usuario).Where(u => u.ProveedorId == proveedorId);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Observaciones.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO FECHA
            if (dateTimeFilter != null)
            {
                queryable = queryable.Where(q => q.FechaHora.Date.Equals(dateTimeFilter));
            }

            //ORDEN
            IOrderedQueryable<MovimientoCtaCteProveedor> queryableOrdered;
            if (sortColumnIndex == 0)   //FECHA HORA
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.FechaHora.Date).ThenBy(c => c.FechaHora.TimeOfDay)
                                        : queryable.OrderByDescending(c => c.FechaHora.Date).ThenByDescending(c => c.FechaHora.TimeOfDay);
            }
            else if (sortColumnIndex == 3) //IMPORTE (DECIMAL)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.Importe)
                                        : queryable.OrderByDescending(c => c.Importe);
            }
            else //OTROS (STRING)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 1 ? c.Usuario.Nombre :
                                                                    sortColumnIndex == 2 ? c.Tipo :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 1 ? c.Usuario.Nombre :
                                                                    sortColumnIndex == 2 ? c.Tipo :
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

        public async Task<decimal> ObtenerDisponibleActualCtaCteProveedorAsync(int proveedorId)
        {
            var monto = (await _db.Proveedores.Where(c => c.ProveedorId == proveedorId).FirstOrDefaultAsync())?.MontoCtaCte ?? 0;

            var queryable = _db.MovimientosCtaCteProveedor.Where(u => u.ProveedorId == proveedorId);

            var saldo = await queryable.SumAsync(q => (decimal?)(q.Tipo.Equals("C") ? q.Importe : q.Importe * -1) ?? 0);

            return monto + saldo;
        }

        public async Task<MovimientoCtaCteProveedor> GuardarMovimientoCtaCteProveedorAsync(ProveedorCtaCteMovimientoDTO movimientoDto, int usuarioId)
        {
            int proveedorId = EncryptionService.Decrypt<int>(Uri.UnescapeDataString(movimientoDto.EncryptedProveedorId));

            if (movimientoDto.Tipo == "D")
            {
                var saldoActual = await ObtenerDisponibleActualCtaCteProveedorAsync(proveedorId);
                if (saldoActual - movimientoDto.Importe < 0)
                    throw new HandledException($"No es posible realizar el Egreso ya que el importe ingresado ({movimientoDto.Importe.ToString("C2")}) es superior al disponible actual de la cuenta ({saldoActual.ToString("C2")})");
            }

            var movimiento = new MovimientoCtaCteProveedor()
            {
                FechaHora = DateTime.Now,
                Importe = movimientoDto.Importe,
                Observaciones = movimientoDto.Observaciones,
                Tipo = movimientoDto.Tipo,
                UsuarioId = usuarioId,
                ProveedorId = proveedorId
            };

            _db.MovimientosCtaCteProveedor.Add(movimiento);
            await _db.SaveChangesAsync();

            return movimiento;
        }

        public async Task<decimal> ObtenerMontoActualCtaCteProveedorAsync(int proveedorId)
        {
            return (await _db.Proveedores.Where(c => c.ProveedorId == proveedorId).FirstOrDefaultAsync())?.MontoCtaCte ?? 0;
        }
    }
}
