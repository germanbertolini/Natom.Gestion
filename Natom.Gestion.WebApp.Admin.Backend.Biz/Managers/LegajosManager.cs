using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Managers
{
    public class LegajosManager : BaseManager
    {
        public LegajosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerCountAsync(int clienteId)
                    => _db.Dockets
                            .Where(t => t.ClientId == clienteId)
                            .CountAsync();

        public async Task<List<Docket>> ObtenerDataTableAsync(int clienteId, int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Dockets.Include(d => d.Title).Where(u => u.ClientId == clienteId);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.DocketNumber.ToLower().Contains(filter.ToLower())
                                                    || p.FirstName.ToLower().Contains(filter.ToLower())
                                                    || p.LastName.ToLower().Contains(filter.ToLower())
                                                    || p.Title.Name.ToLower().Contains(filter.ToLower())
                                                    || p.DNI.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => q.Active);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => !q.Active);
            }

            //ORDEN
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.DocketNumber :
                                                                    sortColumnIndex == 1 ? c.FirstName :
                                                                    sortColumnIndex == 2 ? c.LastName :
                                                                    sortColumnIndex == 3 ? c.DNI :
                                                                    sortColumnIndex == 4 ? c.Title.Name :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.DocketNumber :
                                                                    sortColumnIndex == 1 ? c.FirstName :
                                                                    sortColumnIndex == 2 ? c.LastName :
                                                                    sortColumnIndex == 3 ? c.DNI :
                                                                    sortColumnIndex == 4 ? c.Title.Name :
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

        public async Task<Docket> GuardarAsync(Docket docketDto)
        {
            Docket docket = null;
            if (docketDto.DocketId == 0) //NUEVO
            {
                if (await _db.Dockets.AnyAsync(m => m.DocketNumber.ToLower().Equals(docketDto.DocketNumber.ToLower()) && m.ClientId == docketDto.ClientId))
                    throw new HandledException("Ya existe un legajo con mismo numero.");

                docketDto.Active = true;

                _db.Dockets.Add(docketDto);
                await _db.SaveChangesAsync();

                docket = docketDto;
            }
            else //EDICION
            {
                if (await _db.Dockets.AnyAsync(m => m.DocketNumber.ToLower().Equals(docketDto.DocketNumber.ToLower()) && m.ClientId == docketDto.ClientId && m.DocketId != docketDto.DocketId))
                    throw new HandledException("Ya existe un legajo con mismo numero.");

                docket = await _db.Dockets
                                    .Include(d => d.Ranges)
                                    .FirstAsync(u => u.DocketId.Equals(docketDto.DocketId));

                _db.Entry(docket).State = EntityState.Modified;
                docket.DocketNumber = docketDto.DocketNumber;
                docket.FirstName = docketDto.FirstName;
                docket.LastName = docketDto.LastName;
                docket.DNI = docketDto.DNI;
                docket.TitleId = docketDto.TitleId;
                docket.HourValue = docketDto.HourValue;
                docket.ExtraHourValue = docketDto.ExtraHourValue;

                if (docket.Ranges.Count > 0)
                {
                    for (int i = 0; i < docket.Ranges.Count; i++)
                        _db.Entry(docket.Ranges[i]).State = EntityState.Deleted;

                    if (docketDto.Ranges.Count > 0)
                    {
                        foreach (var range in docketDto.Ranges)
                        {
                            if (range.DocketId == 0)
                                docket.Ranges.Add(range);
                            else
                            {
                                var existent = docket.Ranges.First(r => r.DocketRangeId == range.DocketRangeId);
                                _db.Entry(existent).State = EntityState.Modified;
                                existent.From = range.From;
                                existent.To = range.To;
                            }
                        }
                    }
                }
                else if (docketDto.Ranges.Count > 0)
                    docket.Ranges.AddRange(docketDto.Ranges);

                await _db.SaveChangesAsync();
            }

            return docket;
        }

        public Task<List<Docket>> ObtenerActivosAsync(int clienteId)
        {
            return _db.Dockets.Where(t => t.ClientId == clienteId).Where(m => m.Active).ToListAsync();
        }

        public async Task DesactivarAsync(int docketId)
        {
            var docket = await _db.Dockets
                                    .FirstAsync(u => u.DocketId.Equals(docketId));

            _db.Entry(docket).State = EntityState.Modified;
            docket.Active = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarAsync(int docketId)
        {
            var docket = await _db.Dockets
                                    .FirstAsync(u => u.DocketId.Equals(docketId));

            _db.Entry(docket).State = EntityState.Modified;
            docket.Active = true;

            await _db.SaveChangesAsync();
        }

        public Task<Docket> ObtenerAsync(int docketId)
                        => _db.Dockets
                                .Include(d => d.Title)
                                .Include(d => d.Ranges)
                                .FirstAsync(u => u.DocketId.Equals(docketId));

        public Task<List<Docket>> BuscarAsync(int size, string filter)
        {
            var queryable = _db.Dockets.Include(u => u.Title).Where(u => u.Active == true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.Split(' ').Select(w => w.Trim().ToLower());
                foreach (var word in words)
                {
                    queryable = queryable.Where(p => (
                                                        (p.FirstName + " " + p.LastName).ToLower().Contains(word)
                                                    )
                                                    || p.DocketNumber.ToLower().Contains(word));
                }
            }

            //ORDEN
            var queryableOrdered = queryable.OrderBy(c => c.DocketNumber);

            //TAKE
            return queryableOrdered
                    .Take(size)
                    .ToListAsync();
        }
    }
}
