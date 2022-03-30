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
    public class PlacesManager : BaseManager
    {
        public PlacesManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerCountAsync(int clienteId)
                    => _db.Places
                            .Where(t => t.ClientId == clienteId)
                            .CountAsync();

        public async Task<List<Place>> ObtenerDataTableAsync(int clienteId, int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Places
                                    .Include(p => p.Goals)
                                    .Include(p => p.ConfigTolerancias)
                                    .Where(u => u.ClientId == clienteId);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Name.ToLower().Contains(filter.ToLower())
                                                    || p.Address.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => !q.RemovedAt.HasValue);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => q.RemovedAt.HasValue);
            }

            //ORDEN
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.Name :
                                                                    sortColumnIndex == 1 ? c.Address :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.Name :
                                                                            sortColumnIndex == 1 ? c.Address :
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

        public Task<List<Place>> GetPlacesWithoutHoursAsync(int clienteId)
        {
            return _db.Places.Where(p => !p.RemovedAt.HasValue && p.ConfigTolerancias.Count == 0).ToListAsync();
        }

        public async Task<Place> GuardarAsync(Place placeDto)
        {
            Place place = null;
            if (placeDto.PlaceId == 0) //NUEVO
            {
                if (await _db.Places.AnyAsync(m => m.Name.ToLower().Equals(placeDto.Name.ToLower()) && m.ClientId == placeDto.ClientId))
                    throw new HandledException("Ya existe una Planta / Oficina con mismo Nombre.");

                place = new Place()
                {
                    Name = placeDto.Name,
                    ClientId = placeDto.ClientId,
                    Address = placeDto.Address,
                    Lat = placeDto.Lat,
                    Lng = placeDto.Lng   
            };

                _db.Places.Add(place);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                if (await _db.Places.AnyAsync(m => m.Name.ToLower().Equals(placeDto.Name.ToLower()) && m.ClientId == placeDto.ClientId && m.PlaceId != placeDto.PlaceId))
                    throw new HandledException("Ya existe una Planta / Oficina con mismo Nombre.");

                place = await _db.Places
                                    .FirstAsync(u => u.PlaceId.Equals(placeDto.PlaceId));

                _db.Entry(place).State = EntityState.Modified;
                place.Name = placeDto.Name;
                place.Address = placeDto.Address;
                place.Lat = placeDto.Lat;
                place.Lng = placeDto.Lng;

                await _db.SaveChangesAsync();
            }

            return place;
        }

        public Task<List<Place>> ObtenerActivasAsync(int clienteId)
        {
            return _db.Places.Where(t => t.ClientId == clienteId).Where(m => !m.RemovedAt.HasValue).ToListAsync();
        }

        public async Task DesactivarAsync(int placeId)
        {
            var place = await _db.Places
                                    .FirstAsync(u => u.PlaceId.Equals(placeId));

            _db.Entry(place).State = EntityState.Modified;
            place.RemovedAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarAsync(int placeId)
        {
            var place = await _db.Places
                                    .FirstAsync(u => u.PlaceId.Equals(placeId));

            _db.Entry(place).State = EntityState.Modified;
            place.RemovedAt = null;

            await _db.SaveChangesAsync();
        }

        public Task<Place> ObtenerAsync(int placeId)
                        => _db.Places
                                .FirstAsync(u => u.PlaceId.Equals(placeId));
    }
}
