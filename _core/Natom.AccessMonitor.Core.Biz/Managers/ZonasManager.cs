using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.AccessMonitor.Core.Biz.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Managers
{
    public class ZonasManager : BaseManager
    {
        public ZonasManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerZonasCountAsync()
                    => _db.Zonas
                            .CountAsync();

        public async Task<List<Zona>> ObtenerZonasDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Zonas.Where(u => true);

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

        public async Task<Zona> GuardarZonaAsync(Zona zonaDto)
        {
            Zona zona = null;
            if (zonaDto.ZonaId == 0) //NUEVO
            {
                if (await _db.Zonas.AnyAsync(m => m.Descripcion.ToLower().Equals(zonaDto.Descripcion.ToLower())))
                    throw new HandledException("Ya existe una Zona con misma descripción.");

                zona = new Zona()
                {
                    Descripcion = zonaDto.Descripcion,
                    Activo = true
                };

                _db.Zonas.Add(zona);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                if (await _db.Zonas.AnyAsync(m => m.Descripcion.ToLower().Equals(zonaDto.Descripcion.ToLower()) && m.ZonaId != zonaDto.ZonaId))
                    throw new HandledException("Ya existe una Zona con misma descripción.");

                zona = await _db.Zonas
                                    .FirstAsync(u => u.ZonaId.Equals(zonaDto.ZonaId));

                _db.Entry(zona).State = EntityState.Modified;
                zona.Descripcion = zonaDto.Descripcion;

                await _db.SaveChangesAsync();
            }

            return zona;
        }

        public Task<List<Zona>> ObtenerZonasActivasAsync()
        {
            return _db.Zonas.Where(m => m.Activo).ToListAsync();
        }

        public async Task DesactivarZonaAsync(int zonaId)
        {
            var zona = await _db.Zonas
                                    .FirstAsync(u => u.ZonaId.Equals(zonaId));

            _db.Entry(zona).State = EntityState.Modified;
            zona.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarZonaAsync(int zonaId)
        {
            var zona = await _db.Zonas
                                    .FirstAsync(u => u.ZonaId.Equals(zonaId));

            _db.Entry(zona).State = EntityState.Modified;
            zona.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<Zona> ObtenerZonaAsync(int zonaId)
                        => _db.Zonas
                                .FirstAsync(u => u.ZonaId.Equals(zonaId));
    }
}
