using Microsoft.EntityFrameworkCore;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Transportes;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Zonas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class TransportesManager : BaseManager
    {
        public TransportesManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerTransportesCountAsync()
                    => _db.Transportes
                            .CountAsync();

        public async Task<List<Transporte>> ObtenerTransportesDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Transportes.Where(u => true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Descripcion.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => q.Activo);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => !q.Activo);
            }

            //ORDEN
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.Descripcion :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.Descripcion :
                                                            "");

            var countFiltrados = queryableOrdered.Count();

            //SKIP Y TAKE
            var result = await queryableOrdered
                    .Skip(start)
                    .Take(size)
                    .ToListAsync();

            result.ForEach(r => r.CantidadFiltrados = countFiltrados);

            return result;
        }

        public async Task<Transporte> GuardarTransporteAsync(TransporteDTO transporteDto)
        {
            Transporte transporte = null;
            if (string.IsNullOrEmpty(transporteDto.EncryptedId)) //NUEVO
            {
                if (await _db.Transportes.AnyAsync(m => m.Descripcion.ToLower().Equals(transporteDto.Descripcion.ToLower())))
                    throw new HandledException("Ya existe un Transporte con misma descripción.");

                transporte = new Transporte()
                {
                    Descripcion = transporteDto.Descripcion,
                    Activo = true
                };

                _db.Transportes.Add(transporte);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int transporteId = EncryptionService.Decrypt<int>(transporteDto.EncryptedId);

                if (await _db.Transportes.AnyAsync(m => m.Descripcion.ToLower().Equals(transporteDto.Descripcion.ToLower()) && m.TransporteId != transporteId))
                    throw new HandledException("Ya existe una Transporte con misma descripción.");

                transporte = await _db.Transportes
                                    .FirstAsync(u => u.TransporteId.Equals(transporteId));

                _db.Entry(transporte).State = EntityState.Modified;
                transporte.Descripcion = transporteDto.Descripcion;

                await _db.SaveChangesAsync();
            }

            return transporte;
        }

        public Task<List<Transporte>> ObtenerTransportesActivasAsync()
        {
            return _db.Transportes.Where(m => m.Activo).ToListAsync();
        }

        public async Task DesactivarTransporteAsync(int transporteId)
        {
            var transporte = await _db.Transportes
                                    .FirstAsync(u => u.TransporteId.Equals(transporteId));

            _db.Entry(transporte).State = EntityState.Modified;
            transporte.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarTransporteAsync(int transporteId)
        {
            var transporte = await _db.Transportes
                                    .FirstAsync(u => u.TransporteId.Equals(transporteId));

            _db.Entry(transporte).State = EntityState.Modified;
            transporte.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<Transporte> ObtenerTransporteAsync(int transporteId)
                        => _db.Transportes
                                .FirstAsync(u => u.TransporteId.Equals(transporteId));
    }
}
