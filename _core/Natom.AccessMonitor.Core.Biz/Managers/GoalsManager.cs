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
    public class GoalsManager : BaseManager
    {
        public GoalsManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerCountAsync(int clienteId)
                    => _db.Goals
                            .Where(t => t.PlaceId == clienteId)
                            .CountAsync();

        public async Task<List<Goal>> ObtenerDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, int placeId, string statusFilter)
        {
            var queryable = _db.Goals
                                .Include(g => g.Place)
                                .Where(u => u.PlaceId == placeId);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Name.ToLower().Contains(filter.ToLower())
                                                    || p.Address.ToLower().Contains(filter.ToLower())
                                                    || p.Place.Name.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => !q.RemovedAt.HasValue && !q.Place.RemovedAt.HasValue);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => q.RemovedAt.HasValue || q.Place.RemovedAt.HasValue);
            }

            //ORDEN
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.Name :
                                                                    sortColumnIndex == 1 ? c.Address :
                                                                    sortColumnIndex == 2 ? c.Place.Name :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.Name :
                                                                            sortColumnIndex == 1 ? c.Address :
                                                                            sortColumnIndex == 2 ? c.Place.Name :
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

        public async Task<Goal> GuardarAsync(int clientId, Goal goalDto)
        {
            Goal goal = null;
            if (goalDto.GoalId == 0) //NUEVO
            {
                if (await _db.Goals.AnyAsync(m => m.Name.ToLower().Equals(goalDto.Name.ToLower()) && m.Place.ClientId == clientId))
                    throw new HandledException("Ya existe una Portería / Acceso con mismo Nombre.");

                goal = new Goal()
                {
                    Name = goalDto.Name,
                    Address = goalDto.Address,
                    PlaceId = goalDto.PlaceId,
                    Lat = goalDto.Lat,
                    Lng = goalDto.Lng
            };

                _db.Goals.Add(goal);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                if (await _db.Goals.AnyAsync(m => m.Name.ToLower().Equals(goalDto.Name.ToLower()) && m.Place.ClientId == clientId && m.GoalId != goalDto.GoalId))
                    throw new HandledException("Ya existe una Portería / Acceso con mismo Nombre.");

                goal = await _db.Goals
                                    .FirstAsync(u => u.GoalId.Equals(goalDto.GoalId));

                _db.Entry(goal).State = EntityState.Modified;
                goal.Name = goalDto.Name;
                goal.Address = goalDto.Address;
                goal.PlaceId = goalDto.PlaceId;
                goal.Lat = goalDto.Lat;
                goal.Lng = goalDto.Lng;

                await _db.SaveChangesAsync();
            }

            return goal;
        }

        public Task<List<Goal>> ObtenerActivasAsync(int clienteId)
        {
            return _db.Goals.Include(t => t.Place).Where(t => t.Place.ClientId == clienteId).Where(m => !m.Place.RemovedAt.HasValue && !m.RemovedAt.HasValue).ToListAsync();
        }

        public async Task DesactivarAsync(int goalId)
        {
            var goal = await _db.Goals
                                    .FirstAsync(u => u.GoalId.Equals(goalId));

            _db.Entry(goal).State = EntityState.Modified;
            goal.RemovedAt = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarAsync(int goalId)
        {
            var goal = await _db.Goals
                                    .FirstAsync(u => u.GoalId.Equals(goalId));

            _db.Entry(goal).State = EntityState.Modified;
            goal.RemovedAt = null;

            await _db.SaveChangesAsync();
        }

        public Task<Goal> ObtenerAsync(int goalId)
                        => _db.Goals
                                .Include(g => g.Place)
                                .FirstAsync(u => u.GoalId.Equals(goalId));
    }
}
