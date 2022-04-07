using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Productos;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class CategoriasProductosManager : BaseManager
    {
        public CategoriasProductosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerCategoriasProductosCountAsync()
                    => _db.CategoriasProducto
                            .CountAsync();

        public async Task<List<CategoriaProducto>> ObtenerCategoriasProductosDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.CategoriasProducto.Where(u => true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.Descripcion.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => !q.Eliminado);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => q.Eliminado);
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

        public async Task<CategoriaProducto> GuardarCategoriaProductoAsync(CategoriaProductoDTO categoriaProductoDto)
        {
            CategoriaProducto categoriaProducto = null;
            if (string.IsNullOrEmpty(categoriaProductoDto.EncryptedId)) //NUEVO
            {
                if (await _db.CategoriasProducto.AnyAsync(m => m.Descripcion.ToLower().Equals(categoriaProductoDto.Descripcion.ToLower())))
                    throw new HandledException("Ya existe una Categoria de producto con misma descripción.");

                categoriaProducto = new CategoriaProducto()
                {
                    Descripcion = categoriaProductoDto.Descripcion,
                    Eliminado = false
                };

                _db.CategoriasProducto.Add(categoriaProducto);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int categoriaProductoId = EncryptionService.Decrypt<int>(categoriaProductoDto.EncryptedId);

                if (await _db.CategoriasProducto.AnyAsync(m => m.Descripcion.ToLower().Equals(categoriaProductoDto.Descripcion.ToLower()) && m.CategoriaProductoId != categoriaProductoId))
                    throw new HandledException("Ya existe una Categoria de producto con misma descripción.");

                categoriaProducto = await _db.CategoriasProducto
                                    .FirstAsync(u => u.CategoriaProductoId.Equals(categoriaProductoId));

                _db.Entry(categoriaProducto).State = EntityState.Modified;
                categoriaProducto.Descripcion = categoriaProductoDto.Descripcion;

                await _db.SaveChangesAsync();
            }

            return categoriaProducto;
        }

        public Task<List<CategoriaProducto>> ObtenerCategoriasProductosActivasAsync()
        {
            return _db.CategoriasProducto.Where(m => !m.Eliminado).ToListAsync();
        }

        public async Task DesactivarCategoriaProductoAsync(int categoriaProductoId)
        {
            var categoriaProducto = await _db.CategoriasProducto
                                    .FirstAsync(u => u.CategoriaProductoId.Equals(categoriaProductoId));

            _db.Entry(categoriaProducto).State = EntityState.Modified;
            categoriaProducto.Eliminado = true;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarCategoriaProductoAsync(int categoriaProductoId)
        {
            var categoriaProducto = await _db.CategoriasProducto
                                    .FirstAsync(u => u.CategoriaProductoId.Equals(categoriaProductoId));

            _db.Entry(categoriaProducto).State = EntityState.Modified;
            categoriaProducto.Eliminado = false;

            await _db.SaveChangesAsync();
        }

        public Task<CategoriaProducto> ObtenerCategoriaProductoAsync(int categoriaProductoId)
                        => _db.CategoriasProducto
                                .FirstAsync(u => u.CategoriaProductoId.Equals(categoriaProductoId));
    }
}
