using Microsoft.EntityFrameworkCore;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Precios;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class PreciosManager : BaseManager
    {
        public PreciosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerPreciosCountAsync()
                    => _db.ProductosPrecios
                            .CountAsync();

        public List<spPreciosListResult> ObtenerPreciosDataTable(int start, int size, string filter, int sortColumnIndex, string sortDirection, int? listaDePreciosIdFilter)
        {
            var queryable = _db.spPreciosListResult.FromSqlRaw("spPreciosList {0}", listaDePreciosIdFilter).AsEnumerable();

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.ProductoDescripcion.ToLower().Contains(filter.ToLower())
                                                    || p.ListaDePrecioDescripcion.ToLower().Contains(filter.ToLower()));
            }

            //ORDEN
            IOrderedEnumerable<spPreciosListResult> queryableOrdered;
            if (sortColumnIndex == 2)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.AplicaDesdeFechaHora)
                                        : queryable.OrderByDescending(c => c.AplicaDesdeFechaHora);
            }
            else if (sortColumnIndex == 3)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.Precio)
                                        : queryable.OrderByDescending(c => c.Precio);
            }
            else
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.ProductoDescripcion :
                                                                    sortColumnIndex == 1 ? c.ListaDePrecioDescripcion :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.ProductoDescripcion :
                                                                    sortColumnIndex == 1 ? c.ListaDePrecioDescripcion :
                                                            "");
            }

            var countFiltrados = queryableOrdered.Count();

            //SKIP Y TAKE
            var result = queryableOrdered
                    .Skip(start)
                    .Take(size)
                    .ToList();

            result.ForEach(r => r.CantidadFiltrados = countFiltrados);

            return result;
        }

        public async Task<ProductoPrecio> GuardarPrecioAsync(PrecioDTO precioDto)
        {
            ProductoPrecio precio = new ProductoPrecio()
            {
                AplicaDesdeFechaHora = DateTime.Now,
                ListaDePreciosId = EncryptionService.Decrypt<int>(precioDto.ListaDePreciosEncryptedId),
                Precio = precioDto.Precio,
                ProductoId = EncryptionService.Decrypt<int>(precioDto.ProductoEncryptedId)
            };

            _db.ProductosPrecios.Add(precio);
            await _db.SaveChangesAsync();

            return precio;
        }

        public decimal? ObtenerPrecioActual(int productoId, int listaDePreciosId)
        {
            return _db.spPrecioGetResult.FromSqlRaw("spPrecioGet {0}, {1}", listaDePreciosId, productoId).AsEnumerable().FirstOrDefault()?.Precio;
        }

        public List<spPrecioGetResult> ObtenerPreciosActuales(int productoId, int? listaDePreciosId = null)
        {
            return _db.spPrecioGetResult.FromSqlRaw("spPrecioGet {0}, {1}", listaDePreciosId, productoId).AsEnumerable().ToList();
        }

        public Task<ProductoPrecio> ObtenerPrecioAsync(int productoPrecioId)
        {
            return _db.ProductosPrecios
                            .Include(p => p.ListaDePrecios)
                            .Include(p => p.Producto)
                            .FirstAsync(p => p.ProductoPrecioId.Equals(productoPrecioId));
        }

        public Task<List<ListaDePrecios>> ObtenerListasDePreciosAsync()
        {
            return _db.ListasDePrecios.ToListAsync();
        }

        public Task<List<HistoricoReajustePrecio>> ObtenerPreciosReajustesDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter, string listaFilter)
        {
            var queryable = _db.HistoricosReajustePrecios
                                    .Include(r => r.AplicoMarca)
                                    .Include(r => r.AplicoListaDePrecios)
                                    .Include(r => r.Usuario)
                                    .Where(u => true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.AplicoMarca.Descripcion.ToLower().Contains(filter.ToLower())
                                                    || p.AplicoListaDePrecios.Descripcion.ToLower().Contains(filter.ToLower())
                                                    || p.Usuario.Nombre.ToLower().Contains(filter.ToLower()));
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => !q.FechaHoraBaja.HasValue);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => q.FechaHoraBaja.HasValue);
            }

            //FILTRO DE LISTA DE PRECIOS
            if (!string.IsNullOrEmpty(listaFilter))
            {
                int listaDePreciosId = EncryptionService.Decrypt<int>(listaFilter);
                queryable = queryable.Where(p => p.AplicoListaDePreciosId.Equals(listaDePreciosId));
            }

            //ORDEN
            IOrderedQueryable<HistoricoReajustePrecio> queryableOrdered;
            if (sortColumnIndex == 2)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.EsPorcentual).ThenBy(c => c.Valor)
                                        : queryable.OrderByDescending(c => c.EsPorcentual).ThenByDescending(c => c.Valor);
            }
            else if (sortColumnIndex == 4)
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => c.AplicaDesdeFechaHora)
                                        : queryable.OrderByDescending(c => c.AplicaDesdeFechaHora);
            }
            else
            {
                queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0 ? c.AplicoMarca.Descripcion :
                                                                    sortColumnIndex == 3 ? c.AplicoListaDePrecios.Descripcion :
                                                                    sortColumnIndex == 5 ? c.Usuario.Nombre :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0 ? c.AplicoMarca.Descripcion :
                                                                    sortColumnIndex == 3 ? c.AplicoListaDePrecios.Descripcion :
                                                                    sortColumnIndex == 5 ? c.Usuario.Nombre :
                                                            "");
            }
            

            //SKIP Y TAKE
            return queryableOrdered
                    .Skip(start)
                    .Take(size)
                    .ToListAsync();
        }

        public Task<int> ObtenerPreciosReajustesCountAsync()
        {
            return _db.HistoricosReajustePrecios.CountAsync();
        }

        public Task<HistoricoReajustePrecio> ObtenerPreciosReajusteAsync(int reajustePrecioId)
        {
            return _db.HistoricosReajustePrecios
                        .FirstAsync(r => r.HistoricoReajustePrecioId == reajustePrecioId);
        }

        public async Task<HistoricoReajustePrecio> GuardarReajustePrecioAsync(int usuarioId, PrecioReajusteDTO precioReajusteDto)
        {
            var todasLasListasDePreciosIds = await _db.ListasDePrecios.Where(l => l.Activo).Select(l => l.ListaDePreciosId).ToListAsync();
            var listasDePreciosId = precioReajusteDto.AplicoListaDePreciosEncryptedId == "-1" ? todasLasListasDePreciosIds : new List<int> { EncryptionService.Decrypt<int>(precioReajusteDto.AplicoListaDePreciosEncryptedId) };
            var marcaId = EncryptionService.Decrypt<int>(precioReajusteDto.AplicoMarcaEncryptedId);
            var preciosActuales = await _db.ProductosPrecios
                                            .Where(p => listasDePreciosId.Contains(p.ListaDePreciosId.Value)
                                                            && p.Producto.MarcaId == marcaId
                                                            && !p.FechaHoraBaja.HasValue)
                                            .ToListAsync();

            var reajustePrecio = new HistoricoReajustePrecio
            {
                AplicaDesdeFechaHora = DateTime.Now,
                AplicoListaDePreciosId = precioReajusteDto.AplicoListaDePreciosEncryptedId == "-1" ? null /*TODAS*/ : EncryptionService.Decrypt<int>(precioReajusteDto.AplicoListaDePreciosEncryptedId),
                AplicoMarcaId = marcaId,
                EsIncremento = precioReajusteDto.EsIncremento,
                EsPorcentual = precioReajusteDto.EsPorcentual,
                FechaHora = DateTime.Now,
                UsuarioId = usuarioId,
                Valor = precioReajusteDto.Valor
            };
            _db.HistoricosReajustePrecios.Add(reajustePrecio);
            await _db.SaveChangesAsync();

            foreach (var actual in preciosActuales)
            {
                var nuevoPrecio = new ProductoPrecio
                {
                    AplicaDesdeFechaHora = reajustePrecio.AplicaDesdeFechaHora,
                    ListaDePreciosId = actual.ListaDePreciosId,
                    ProductoId = actual.ProductoId,
                    HistoricoReajustePrecioId = reajustePrecio.HistoricoReajustePrecioId
                };

                if (reajustePrecio.EsIncremento && reajustePrecio.EsPorcentual)
                    nuevoPrecio.Precio = actual.Precio * ((reajustePrecio.Valor / 100) + 1);
                else if (reajustePrecio.EsIncremento && !reajustePrecio.EsPorcentual)
                    nuevoPrecio.Precio = actual.Precio + reajustePrecio.Valor;
                else if (!reajustePrecio.EsIncremento && reajustePrecio.EsPorcentual)
                    nuevoPrecio.Precio = actual.Precio / ((reajustePrecio.Valor / 100) + 1);
                else if (!reajustePrecio.EsIncremento && !reajustePrecio.EsPorcentual)
                    nuevoPrecio.Precio = actual.Precio - reajustePrecio.Valor;

                _db.ProductosPrecios.Add(nuevoPrecio);
            }
            await _db.SaveChangesAsync();

            return reajustePrecio;
        }

        public async Task EliminarReajusteAsync(int reajustePreciosId)
        {
            var reajuste = await _db.HistoricosReajustePrecios.FindAsync(reajustePreciosId);
            var precios = await _db.ProductosPrecios.Where(p => p.HistoricoReajustePrecioId == reajustePreciosId).ToListAsync();

            _db.Entry(reajuste).State = EntityState.Modified;
            reajuste.FechaHoraBaja = DateTime.Now;

            foreach (var precio in precios)
            {
                _db.Entry(precio).State = EntityState.Modified;
                precio.FechaHoraBaja = reajuste.FechaHoraBaja;
            }

            await _db.SaveChangesAsync();
        }
    }
}
