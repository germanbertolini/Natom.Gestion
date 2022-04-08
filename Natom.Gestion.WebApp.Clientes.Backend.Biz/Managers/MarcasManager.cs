using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Marcas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class MarcasManager : BaseManager
    {
        public MarcasManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerMarcasCountAsync()
                    => _db.Marcas
                            .CountAsync();

        public async Task<List<Marca>> ObtenerMarcasDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Marcas.Where(u => true);

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

        public async Task<Marca> GuardarMarcaAsync(MarcaDTO marcaDto)
        {
            Marca marca = null;
            if (string.IsNullOrEmpty(marcaDto.EncryptedId)) //NUEVO
            {
                if (await _db.Marcas.AnyAsync(m => m.Descripcion.ToLower().Equals(marcaDto.Descripcion.ToLower())))
                    throw new HandledException("Ya existe una Marca con misma descripción.");

                marca = new Marca()
                {
                    Descripcion = marcaDto.Descripcion,
                    Activo = true
                };

                _db.Marcas.Add(marca);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int marcaId = EncryptionService.Decrypt<int, Marca>(marcaDto.EncryptedId);

                if (await _db.Marcas.AnyAsync(m => m.Descripcion.ToLower().Equals(marcaDto.Descripcion.ToLower()) && m.MarcaId != marcaId))
                    throw new HandledException("Ya existe una Marca con misma descripción.");

                marca = await _db.Marcas
                                    .FirstAsync(u => u.MarcaId.Equals(marcaId));

                _db.Entry(marca).State = EntityState.Modified;
                marca.Descripcion = marcaDto.Descripcion;

                await _db.SaveChangesAsync();
            }

            return marca;
        }

        public Task<List<Marca>> ObtenerMarcasActivasAsync()
        {
            return _db.Marcas.Where(m => m.Activo).OrderBy(m => m.Descripcion).ToListAsync();
        }

        public async Task DesactivarMarcaAsync(int marcaId)
        {
            var marca = await _db.Marcas
                                    .FirstAsync(u => u.MarcaId.Equals(marcaId));

            _db.Entry(marca).State = EntityState.Modified;
            marca.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarMarcaAsync(int marcaId)
        {
            var marca = await _db.Marcas
                                    .FirstAsync(u => u.MarcaId.Equals(marcaId));

            _db.Entry(marca).State = EntityState.Modified;
            marca.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<Marca> ObtenerMarcaAsync(int marcaId)
                        => _db.Marcas
                                .FirstAsync(u => u.MarcaId.Equals(marcaId));
    }
}
