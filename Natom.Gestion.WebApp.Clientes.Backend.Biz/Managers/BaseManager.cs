using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Natom.Extensions.Auth.Repository;
using Natom.Extensions.Auth.Services;
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
        protected AuthService _authService;
        protected BizDbContext _db;

        public BaseManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _db = (BizDbContext)_serviceProvider.GetService(typeof(BizDbContext));
            _configuration = (IConfiguration)serviceProvider.GetService(typeof(IConfiguration));
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
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

        public async Task<List<HistoricoCambios>> ConsultarHistoricoCambiosAsync(int entityId, string entityName)
        {
            var cambios = await _db.HistoricosCambios
                                    .Where(h => h.EntityName.Equals(entityName) && h.EntityId.Equals(entityId))
                                    .ToListAsync();

            var usuariosIds = cambios.Where(c => c.UsuarioId.HasValue).Select(c => c.UsuarioId.Value).ToList();
            var usuarios = await _authService.ListUsersByIds(usuariosIds);
            cambios.ForEach(d => d.Usuario = usuarios.FirstOrDefault(u => u.UsuarioId == d.UsuarioId));

            return cambios;
        }
    }
}
