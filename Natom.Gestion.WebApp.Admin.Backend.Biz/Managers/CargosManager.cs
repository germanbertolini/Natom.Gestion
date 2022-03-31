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
    public class CargosManager : BaseManager
    {
        public CargosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerCountAsync(int clienteId)
                    => _db.Titles
                            .Where(t => t.ClienteId == clienteId)
                            .CountAsync();

        public async Task<List<Title>> ObtenerDataTableAsync(int clienteId, int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Titles.Where(u => u.ClienteId == clienteId);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Name.ToLower().Contains(filter.ToLower()));
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
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.Name :
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

        public async Task<Title> GuardarAsync(Title titleDto)
        {
            Title title = null;
            if (titleDto.TitleId == 0) //NUEVO
            {
                if (await _db.Titles.AnyAsync(m => m.Name.ToLower().Equals(titleDto.Name.ToLower()) && m.ClienteId == titleDto.ClienteId))
                    throw new HandledException("Ya existe un cargo con mismo Nombre / Descripción.");

                title = new Title()
                {
                    Name = titleDto.Name,
                    ClienteId = titleDto.ClienteId
                };

                _db.Titles.Add(title);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                if (await _db.Titles.AnyAsync(m => m.Name.ToLower().Equals(titleDto.Name.ToLower()) && m.ClienteId == titleDto.ClienteId && m.TitleId != titleDto.TitleId))
                    throw new HandledException("Ya existe un cargo con mismo Nombre / Descripción.");

                title = await _db.Titles
                                    .FirstAsync(u => u.TitleId.Equals(titleDto.TitleId));

                _db.Entry(title).State = EntityState.Modified;
                title.Name = titleDto.Name;

                await _db.SaveChangesAsync();
            }

            return title;
        }

        public Task<List<Title>> ObtenerActivasAsync(int clienteId)
        {
            return _db.Titles.Where(t => t.ClienteId == clienteId).Where(m => !m.RemovedAt.HasValue).ToListAsync();
        }

        public async Task DesactivarAsync(int titleId)
        {
            var title = await _db.Titles
                                    .FirstAsync(u => u.TitleId.Equals(titleId));

            _db.Entry(title).State = EntityState.Modified;
            title.RemovedAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarAsync(int titleId)
        {
            var title = await _db.Titles
                                    .FirstAsync(u => u.TitleId.Equals(titleId));

            _db.Entry(title).State = EntityState.Modified;
            title.RemovedAt = null;

            await _db.SaveChangesAsync();
        }

        public Task<Title> ObtenerAsync(int titleId)
                        => _db.Titles
                                .FirstAsync(u => u.TitleId.Equals(titleId));
    }
}
