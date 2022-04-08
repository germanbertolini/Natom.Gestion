using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Precios;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class ListasDePreciosManager : BaseManager
    {
        public ListasDePreciosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerListasDePreciosCountAsync()
                    => _db.ListasDePrecios
                            .CountAsync();

        public async Task<List<ListaDePrecios>> ObtenerListasDePreciosDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.ListasDePrecios.Where(u => true);

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

        public async Task<ListaDePrecios> GuardarListaDePrecioAsync(ListaDePreciosDTO listaDePrecioDto)
        {
            ListaDePrecios listaDePrecio = null;
            if (string.IsNullOrEmpty(listaDePrecioDto.EncryptedId)) //NUEVO
            {
                if (await _db.ListasDePrecios.AnyAsync(m => m.Descripcion.ToLower().Equals(listaDePrecioDto.Descripcion.ToLower())))
                    throw new HandledException("Ya existe una ListaDePrecio con misma descripción.");

                listaDePrecio = new ListaDePrecios()
                {
                    Descripcion = listaDePrecioDto.Descripcion,
                    Activo = true
                };

                _db.ListasDePrecios.Add(listaDePrecio);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int listaDePrecioId = EncryptionService.Decrypt<int, ListaDePrecios>(listaDePrecioDto.EncryptedId);

                if (await _db.ListasDePrecios.AnyAsync(m => m.Descripcion.ToLower().Equals(listaDePrecioDto.Descripcion.ToLower()) && m.ListaDePreciosId != listaDePrecioId))
                    throw new HandledException("Ya existe una ListaDePrecio con misma descripción.");

                listaDePrecio = await _db.ListasDePrecios
                                    .FirstAsync(u => u.ListaDePreciosId.Equals(listaDePrecioId));

                _db.Entry(listaDePrecio).State = EntityState.Modified;
                listaDePrecio.Descripcion = listaDePrecioDto.Descripcion;

                await _db.SaveChangesAsync();
            }

            return listaDePrecio;
        }

        public Task<List<ListaDePrecios>> ObtenerListasDePreciosActivasAsync()
        {
            return _db.ListasDePrecios.Where(m => m.Activo).ToListAsync();
        }

        public async Task DesactivarListaDePrecioAsync(int listaDePrecioId)
        {
            var listaDePrecio = await _db.ListasDePrecios
                                    .FirstAsync(u => u.ListaDePreciosId.Equals(listaDePrecioId));

            _db.Entry(listaDePrecio).State = EntityState.Modified;
            listaDePrecio.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarListaDePrecioAsync(int listaDePrecioId)
        {
            var listaDePrecio = await _db.ListasDePrecios
                                    .FirstAsync(u => u.ListaDePreciosId.Equals(listaDePrecioId));

            _db.Entry(listaDePrecio).State = EntityState.Modified;
            listaDePrecio.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<ListaDePrecios> ObtenerListaDePrecioAsync(int listaDePrecioId)
                        => _db.ListasDePrecios
                                .FirstAsync(u => u.ListaDePreciosId.Equals(listaDePrecioId));
    }
}
