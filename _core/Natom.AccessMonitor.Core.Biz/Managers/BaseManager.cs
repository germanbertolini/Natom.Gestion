using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Configuration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Managers
{
    public class BaseManager
    {
        protected IServiceProvider _serviceProvider;
        protected ConfigurationService _configuration;
        protected MasterDbContext _db;

        public BaseManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _db = (MasterDbContext)_serviceProvider.GetService(typeof(MasterDbContext));
            _configuration = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
        }

        public async Task RegistrarEnHistoricoCambiosAsync(int clienteId, int entityId, string entityName, string accion, int usuarioId)
        {
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC [dbo].[sp_history_insert] @ClientId, @EntityName, @EntityId, @UsuarioId, @Action";
                command.Parameters.Add(new SqlParameter("@ClientId", clienteId));
                command.Parameters.Add(new SqlParameter("@EntityName", entityName));
                command.Parameters.Add(new SqlParameter("@EntityId", entityId));
                command.Parameters.Add(new SqlParameter("@UsuarioId", usuarioId));
                command.Parameters.Add(new SqlParameter("@Action", accion));

                await _db.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
                await _db.Database.CloseConnectionAsync();
            }
        }

        //public Task<List<HistoricoCambios>> ConsultarHistoricoCambiosAsync(int entityId, string entityName)
        //{
        //    return _db.HistoricosCambios
        //                    .Include(h => h.Usuario)
        //                    .Where(h => h.EntityName.Equals(entityName) && h.EntityId.Equals(entityId))
        //                    .ToListAsync();
        //}
    }
}
