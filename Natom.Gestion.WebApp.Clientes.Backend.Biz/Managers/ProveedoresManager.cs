using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Proveedores;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class ProveedoresManager : BaseManager
    {
        public ProveedoresManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public Task<int> ObtenerProveedoresCountAsync()
                    => _db.Proveedores
                            .CountAsync();

        public async Task<List<Proveedor>> ObtenerProveedoresDataTableAsync(int start, int size, string filter, int sortColumnIndex, string sortDirection, string statusFilter)
        {
            var queryable = _db.Proveedores.Include(c => c.TipoDocumento).Where(u => true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                queryable = queryable.Where(p => p.NumeroDocumento.ToLower().Contains(filter.ToLower())
                                                    || p.Localidad.ToLower().Contains(filter.ToLower())
                                                    ||
                                                    (
                                                        p.Nombre != null
                                                            ? (p.Nombre + " " + p.Apellido).ToLower().Contains(filter.ToLower())
                                                            : p.RazonSocial.ToLower().Contains(filter.ToLower()) || p.NombreFantasia.ToLower().Contains(filter.ToLower()))
                                                    );
            }

            //FILTRO DE ESTADO
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.ToUpper().Equals("ACTIVOS")) queryable = queryable.Where(q => q.Activo);
                else if (statusFilter.ToUpper().Equals("INACTIVOS")) queryable = queryable.Where(q => !q.Activo);
            }

            //ORDEN
            var queryableOrdered = sortDirection.ToLower().Equals("asc")
                                        ? queryable.OrderBy(c => sortColumnIndex == 0
                                                                    ? (
                                                                        c.Nombre != null
                                                                            ? (c.Nombre + " " + c.Apellido)
                                                                            : c.RazonSocial
                                                                    ) :
                                                                 sortColumnIndex == 1 ? c.TipoDocumentoId.ToString() :
                                                                 sortColumnIndex == 2 ? c.Localidad :
                                                            "")
                                        : queryable.OrderByDescending(c => sortColumnIndex == 0
                                                                    ? (
                                                                        c.Nombre != null
                                                                            ? (c.Nombre + " " + c.Apellido)
                                                                            : c.RazonSocial
                                                                    ) :
                                                                 sortColumnIndex == 1 ? c.TipoDocumentoId.ToString() :
                                                                 sortColumnIndex == 2 ? c.Localidad :
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

        public async Task<Proveedor> GuardarProveedorAsync(ProveedorDTO proveedorDto)
        {
            Proveedor proveedor = null;
            if (string.IsNullOrEmpty(proveedorDto.EncryptedId)) //NUEVO
            {
                if (await _db.Proveedores.AnyAsync(m => m.NumeroDocumento.ToLower().Equals(proveedorDto.NumeroDocumento.ToLower())))
                    throw new HandledException($"Ya existe un Proveedor con ese {(proveedorDto.EsEmpresa ? "CUIT" : "DNI")}.");

                proveedor = new Proveedor()
                {
                    Nombre = proveedorDto.Nombre,
                    Apellido = proveedorDto.Apellido,
                    RazonSocial = proveedorDto.RazonSocial,
                    NombreFantasia = proveedorDto.NombreFantasia,
                    Domicilio = proveedorDto.Domicilio,
                    EntreCalles = proveedorDto.EntreCalles,
                    Localidad = proveedorDto.Localidad,
                    EsEmpresa = proveedorDto.EsEmpresa,
                    TipoDocumentoId = proveedorDto.EsEmpresa ? 2 : 1,
                    NumeroDocumento = proveedorDto.NumeroDocumento,
                    ContactoEmail1 = proveedorDto.ContactoEmail1,
                    ContactoEmail2 = proveedorDto.ContactoEmail2,
                    ContactoTelefono1 = proveedorDto.ContactoTelefono1,
                    ContactoTelefono2 = proveedorDto.ContactoTelefono2,
                    ContactoObservaciones = proveedorDto.ContactoObservaciones,
                    EsPresupuesto = proveedorDto.EsPresupuesto,
                    MontoCtaCte = proveedorDto.MontoCtaCte,
                    Activo = true
                };

                _db.Proveedores.Add(proveedor);
                await _db.SaveChangesAsync();
            }
            else //EDICION
            {
                int proveedorId = EncryptionService.Decrypt<int, Proveedor>(proveedorDto.EncryptedId);

                if (await _db.Proveedores.AnyAsync(m => m.NumeroDocumento.ToLower().Equals(proveedorDto.NumeroDocumento.ToLower()) && m.ProveedorId != proveedorId))
                    throw new HandledException($"Ya existe un Proveedor con ese {(proveedorDto.EsEmpresa ? "CUIT" : "DNI")}.");

                proveedor = await _db.Proveedores
                                    .FirstAsync(u => u.ProveedorId.Equals(proveedorId));

                _db.Entry(proveedor).State = EntityState.Modified;
                proveedor.Nombre = proveedorDto.Nombre;
                proveedor.Apellido = proveedorDto.Apellido;
                proveedor.RazonSocial = proveedorDto.RazonSocial;
                proveedor.NombreFantasia = proveedorDto.NombreFantasia;
                proveedor.Domicilio = proveedorDto.Domicilio;
                proveedor.EntreCalles = proveedorDto.EntreCalles;
                proveedor.Localidad = proveedorDto.Localidad;
                proveedor.EsEmpresa = proveedorDto.EsEmpresa;
                proveedor.TipoDocumentoId = proveedorDto.EsEmpresa ? 2 : 1;
                proveedor.NumeroDocumento = proveedorDto.NumeroDocumento;
                proveedor.ContactoEmail1 = proveedorDto.ContactoEmail1;
                proveedor.ContactoEmail2 = proveedorDto.ContactoEmail2;
                proveedor.ContactoTelefono1 = proveedorDto.ContactoTelefono1;
                proveedor.ContactoTelefono2 = proveedorDto.ContactoTelefono2;
                proveedor.ContactoObservaciones = proveedorDto.ContactoObservaciones;
                proveedor.EsPresupuesto = proveedorDto.EsPresupuesto;
                proveedor.MontoCtaCte = proveedorDto.MontoCtaCte;

                await _db.SaveChangesAsync();
            }

            return proveedor;
        }

        public async Task DesactivarProveedorAsync(int proveedorId)
        {
            var proveedor = await _db.Proveedores
                                    .FirstAsync(u => u.ProveedorId.Equals(proveedorId));

            _db.Entry(proveedor).State = EntityState.Modified;
            proveedor.Activo = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivarProveedorAsync(int proveedorId)
        {
            var proveedor = await _db.Proveedores
                                    .FirstAsync(u => u.ProveedorId.Equals(proveedorId));

            _db.Entry(proveedor).State = EntityState.Modified;
            proveedor.Activo = true;

            await _db.SaveChangesAsync();
        }

        public Task<Proveedor> ObtenerProveedorAsync(int proveedorId)
                        => _db.Proveedores
                                .Include(d => d.TipoDocumento)
                                .FirstAsync(u => u.ProveedorId.Equals(proveedorId));

        public Task<List<Proveedor>> BuscarProveedoresAsync(int size, string filter)
        {
            var queryable = _db.Proveedores.Include(u => u.TipoDocumento).Where(u => u.Activo == true);

            //FILTROS
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.Split(' ').Select(w => w.Trim().ToLower());
                foreach (var word in words)
                {
                    queryable = queryable.Where(p => (
                                                        p.EsEmpresa ? p.RazonSocial.ToLower().Contains(word)
                                                                    : (p.Nombre + " " + p.Apellido).ToLower().Contains(word)
                                                    )
                                                    || p.NumeroDocumento.ToLower().Contains(word));
                }
            }

            //ORDEN
            var queryableOrdered = queryable.OrderBy(c => c.RazonSocial);

            //TAKE
            return queryableOrdered
                    .Take(size)
                    .ToListAsync();
        }
    }
}
