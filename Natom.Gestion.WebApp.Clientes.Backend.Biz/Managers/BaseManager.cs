using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class BaseManager
    {
        protected IServiceProvider _serviceProvider;
        protected IConfiguration _configuration;
        protected BizDbContext _db;

        public BaseManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _db = (BizDbContext)_serviceProvider.GetService(typeof(BizDbContext));
            _configuration = (IConfiguration)serviceProvider.GetService(typeof(IConfiguration));
        }

        public Task RegistrarEnHistoricoCambiosAsync(int entityId, string entityName, string accion, int usuarioId, string motivo = null)
        {
            var historico = new HistoricoCambios()
            {
                FechaHora = DateTime.Now,
                UsuarioId = usuarioId,
                Accion = accion,
                EntityId = entityId,
                EntityName = entityName
            };
            if (!string.IsNullOrEmpty(motivo))
                historico.Motivos = new List<HistoricoCambiosMotivo>()
                {
                    new HistoricoCambiosMotivo
                    {
                        Motivo = motivo
                    }
                };
            _db.HistoricosCambios.Add(historico);
            return _db.SaveChangesAsync();
        }

        public Task<List<HistoricoCambios>> ConsultarHistoricoCambiosAsync(int entityId, string entityName)
        {
            return _db.HistoricosCambios
                            .Include(h => h.Usuario)
                            .Where(h => h.EntityName.Equals(entityName) && h.EntityId.Equals(entityId))
                            .ToListAsync();
        }
    }
}
