using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Stock;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class DepositosManager : BaseManager
    {
        public DepositosManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerDepositosCountAsync()
                    => _db.Depositos
                            .CountAsync();

        public async Task<List<Deposito>> ObtenerDepositosDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Depositos.Where(u => true);

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

        public async Task<Deposito> GuardarDepositoAsync(DepositoDTO depositoDto)
        {
            Deposito deposito = null;
            if (string.IsNullOrEmpty(depositoDto.EncryptedId)) //NUEVO
            {
                if (await _db.Depositos.AnyAsync(m => m.Descripcion.ToLower().Equals(depositoDto.Descripcion.ToLower())))
                    throw new HandledException("Ya existe una Deposito con misma descripción.");

                deposito = new Deposito()
                {
                    Descripcion = depositoDto.Descripcion,
                    Activo = true
                };

                _db.Depositos.Add(deposito);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int depositoId = EncryptionService.Decrypt<int, Deposito>(depositoDto.EncryptedId);

                if (await _db.Depositos.AnyAsync(m => m.Descripcion.ToLower().Equals(depositoDto.Descripcion.ToLower()) && m.DepositoId != depositoId))
                    throw new HandledException("Ya existe una Deposito con misma descripción.");

                deposito = await _db.Depositos
                                    .FirstAsync(u => u.DepositoId.Equals(depositoId));

                _db.Entry(deposito).State = EntityState.Modified;
                deposito.Descripcion = depositoDto.Descripcion;

                await _db.SaveChangesAsync();
            }

            return deposito;
        }

        public Task<List<Deposito>> ObtenerDepositosActivasAsync()
        {
            return _db.Depositos.Where(m => m.Activo).ToListAsync();
        }

        public async Task DesactivarDepositoAsync(int depositoId)
        {
            var deposito = await _db.Depositos
                                    .FirstAsync(u => u.DepositoId.Equals(depositoId));

            _db.Entry(deposito).State = EntityState.Modified;
            deposito.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarDepositoAsync(int depositoId)
        {
            var deposito = await _db.Depositos
                                    .FirstAsync(u => u.DepositoId.Equals(depositoId));

            _db.Entry(deposito).State = EntityState.Modified;
            deposito.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<Deposito> ObtenerDepositoAsync(int depositoId)
                        => _db.Depositos
                                .FirstAsync(u => u.DepositoId.Equals(depositoId));
    }
}
