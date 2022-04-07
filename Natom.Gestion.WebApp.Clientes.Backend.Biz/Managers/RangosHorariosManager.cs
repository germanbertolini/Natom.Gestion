using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Pedidos;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class RangosHorariosManager : BaseManager
    {
        public RangosHorariosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerRangosHorariosCountAsync()
                    => _db.RangosHorario
                            .CountAsync();

        public async Task<List<RangoHorario>> ObtenerRangosHorariosDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.RangosHorario.Where(u => true);

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

        public async Task<RangoHorario> GuardarRangoHorarioAsync(RangoHorarioDTO rangoHorarioDto)
        {
            RangoHorario rangoHorario = null;
            if (string.IsNullOrEmpty(rangoHorarioDto.EncryptedId)) //NUEVO
            {
                if (await _db.RangosHorario.AnyAsync(m => m.Descripcion.ToLower().Equals(rangoHorarioDto.Descripcion.ToLower())))
                    throw new HandledException("Ya existe una Rango horario con misma descripción.");

                rangoHorario = new RangoHorario()
                {
                    Descripcion = rangoHorarioDto.Descripcion,
                    Activo = true
                };

                _db.RangosHorario.Add(rangoHorario);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int rangoHorarioId = EncryptionService.Decrypt<int>(rangoHorarioDto.EncryptedId);

                if (await _db.RangosHorario.AnyAsync(m => m.Descripcion.ToLower().Equals(rangoHorarioDto.Descripcion.ToLower()) && m.RangoHorarioId != rangoHorarioId))
                    throw new HandledException("Ya existe una Rango horario con misma descripción.");

                rangoHorario = await _db.RangosHorario
                                    .FirstAsync(u => u.RangoHorarioId.Equals(rangoHorarioId));

                _db.Entry(rangoHorario).State = EntityState.Modified;
                rangoHorario.Descripcion = rangoHorarioDto.Descripcion;

                await _db.SaveChangesAsync();
            }

            return rangoHorario;
        }

        public Task<List<RangoHorario>> ObtenerRangosHorariosActivasAsync()
        {
            return _db.RangosHorario.Where(m => m.Activo).ToListAsync();
        }

        public async Task DesactivarRangoHorarioAsync(int rangoHorarioId)
        {
            var rangoHorario = await _db.RangosHorario
                                    .FirstAsync(u => u.RangoHorarioId.Equals(rangoHorarioId));

            _db.Entry(rangoHorario).State = EntityState.Modified;
            rangoHorario.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarRangoHorarioAsync(int rangoHorarioId)
        {
            var rangoHorario = await _db.RangosHorario
                                    .FirstAsync(u => u.RangoHorarioId.Equals(rangoHorarioId));

            _db.Entry(rangoHorario).State = EntityState.Modified;
            rangoHorario.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<RangoHorario> ObtenerRangoHorarioAsync(int rangoHorarioId)
                        => _db.RangosHorario
                                .FirstAsync(u => u.RangoHorarioId.Equals(rangoHorarioId));
    }
}
