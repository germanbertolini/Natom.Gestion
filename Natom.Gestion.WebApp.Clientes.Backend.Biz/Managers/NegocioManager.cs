using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class NegocioManager : BaseManager
    {
        public NegocioManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public NegocioConfig GetConfig()
                    => _db.NegocioConfig.AsEnumerable().FirstOrDefault();

        public NegocioConfig GetCustomConfig(BizDbContext db)
                    => db.NegocioConfig.AsEnumerable().FirstOrDefault();

        public Task SaveConfigAsync(NegocioConfig newConfig)
        {
            var negocio = GetConfig();

            this._db.Entry(negocio).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            negocio.RazonSocial = newConfig.RazonSocial;
            negocio.NombreFantasia = newConfig.NombreFantasia;
            negocio.TipoDocumento = newConfig.TipoDocumento;
            negocio.NumeroDocumento = newConfig.NumeroDocumento;
            negocio.Domicilio = newConfig.Domicilio;
            negocio.Localidad = newConfig.Localidad;
            negocio.Telefono = newConfig.Telefono;
            negocio.Email = newConfig.Email;
            negocio.LogoBase64 = newConfig.LogoBase64;

            return _db.SaveChangesAsync();
        }
    }
}
