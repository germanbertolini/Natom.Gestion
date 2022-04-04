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
    public class ProductosManager : BaseManager
    {
        public ProductosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerProductosCountAsync()
                    => _db.Productos
                            .CountAsync();

        public Task<List<Producto>> BuscarProductosAsync(int size, string filter)
        {
            var queryable = _db.Productos
                                    .Include(p => p.Marca)
                                    .Include(p => p.UnidadPeso)
                                    .Where(u => u.Activo);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.Split(' ').Select(w => w.Trim().ToLower());
                foreach (var word in words)
                {
                    queryable = queryable.Where(p => p.Codigo.ToLower().Contains(word)
                                                    || p.DescripcionCorta.ToLower().Contains(word)
                                                    || p.Marca.Descripcion.ToLower().Contains(word));
                }
            }

            //ORDEN
            var queryableOrdered = queryable.OrderBy(c => c.Codigo);

            //TAKE
            return queryableOrdered
                    .Take(size)
                    .ToListAsync();
        }

        public async Task<List<Producto>> ObtenerProductosDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Productos
                                    .Include(p => p.Marca)
                                    .Include(p => p.UnidadPeso)
                                    .Include(p => p.CategoriaProducto)
                                    .Where(u => true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.Split(' ').Select(w => w.Trim().ToLower());
                foreach (var word in words)
                {
                    queryable = queryable.Where(p => p.Codigo.ToLower().Contains(word)
                                                    || p.DescripcionCorta.ToLower().Contains(word)
                                                    || p.Marca.Descripcion.ToLower().Contains(word)
                                                    || p.CategoriaProducto.Descripcion.ToLower().Contains(word));
                }
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => q.Activo);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => !q.Activo);
            }

            //ORDEN
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.Codigo :
                                                                    sortColumnIndex == 1 ? c.DescripcionCorta :
                                                                    sortColumnIndex == 2 ? c.Marca.Descripcion :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.Codigo :
                                                                            sortColumnIndex == 1 ? c.DescripcionCorta :
                                                                            sortColumnIndex == 2 ? c.Marca.Descripcion :
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

        public async Task<Producto> GuardarProductoAsync(ProductoDTO productoDto)
        {
            Producto producto = null;
            if (string.IsNullOrEmpty(productoDto.EncryptedId)) //NUEVO
            {
                if (!string.IsNullOrEmpty(productoDto.Codigo))
                    if (await _db.Productos.AnyAsync(m => m.Codigo.ToLower().Equals(productoDto.Codigo.ToLower())))
                        throw new HandledException("Ya existe una Producto con mismo código.");

                producto = new Producto()
                {
                    Codigo = productoDto.Codigo?.ToUpper(),
                    DescripcionCorta = productoDto.DescripcionCorta,
                    DescripcionLarga = productoDto.DescripcionLarga,
                    MarcaId = EncryptionService.Decrypt<int>(productoDto.MarcaEncryptedId),
                    MueveStock = productoDto.MueveStock,
                    PesoUnitario = productoDto.PesoUnitario,
                    UnidadPesoId = EncryptionService.Decrypt<int>(productoDto.UnidadPesoEncryptedId),
                    CategoriaProductoId = EncryptionService.Decrypt<string>(productoDto.CategoriaEncryptedId),
                    CostoUnitario = productoDto.CostoUnitario,
                    Activo = true
                };

                _db.Productos.Add(producto);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int productoId = EncryptionService.Decrypt<int>(productoDto.EncryptedId);

                if (!string.IsNullOrEmpty(productoDto.Codigo))
                    if (await _db.Productos.AnyAsync(m => m.Codigo.ToLower().Equals(productoDto.Codigo.ToLower()) && m.ProductoId != productoId))
                        throw new HandledException("Ya existe una Producto con mismo código.");

                producto = await _db.Productos
                                    .FirstAsync(u => u.ProductoId.Equals(productoId));

                _db.Entry(producto).State = EntityState.Modified;
                producto.Codigo = productoDto.Codigo?.ToUpper();
                producto.DescripcionCorta = productoDto.DescripcionCorta;
                producto.DescripcionLarga = productoDto.DescripcionLarga;
                producto.MarcaId = EncryptionService.Decrypt<int>(productoDto.MarcaEncryptedId);
                producto.MueveStock = productoDto.MueveStock;
                producto.PesoUnitario = productoDto.PesoUnitario;
                producto.UnidadPesoId = EncryptionService.Decrypt<int>(productoDto.UnidadPesoEncryptedId);
                producto.CategoriaProductoId = EncryptionService.Decrypt<string>(productoDto.CategoriaEncryptedId);
                producto.CostoUnitario = productoDto.CostoUnitario;

                await _db.SaveChangesAsync();
            }

            return producto;
        }

        public async Task DesactivarProductoAsync(int productoId)
        {
            var producto = await _db.Productos
                                    .FirstAsync(u => u.ProductoId.Equals(productoId));

            _db.Entry(producto).State = EntityState.Modified;
            producto.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarProductoAsync(int productoId)
        {
            var producto = await _db.Productos
                                    .FirstAsync(u => u.ProductoId.Equals(productoId));

            _db.Entry(producto).State = EntityState.Modified;
            producto.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<Producto> ObtenerProductoAsync(int productoId)
                        => _db.Productos
                                .FirstAsync(u => u.ProductoId.Equals(productoId));

        public Task<List<CategoriaProducto>> ObtenerCategoriasActivasAsync()
                        => _db.CategoriasProducto
                                .Where(u => !u.Eliminado)
                                .OrderBy(u => u.CategoriaProductoId)
                                .ToListAsync();

        public Task<List<UnidadPeso>> ObtenerUnidadesPesoAsync()
                        => _db.UnidadesPeso
                                .ToListAsync();
    }
}
