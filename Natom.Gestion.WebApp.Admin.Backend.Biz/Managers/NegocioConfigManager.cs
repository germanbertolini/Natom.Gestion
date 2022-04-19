using Dapper.Contrib.Extensions;
using Natom.Extensions.Configuration.Services;
using Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Managers
{
    public class NegocioConfigManager
    {
        private readonly ConfigurationService _configurationService;

        public NegocioConfigManager(IServiceProvider serviceProvider)
        {
            _configurationService = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
        }

        public async Task SaveConfigAsync(NegocioConfig config, int clienteId)
        {
            var connectionString = _configurationService.GetValueAsync("ConnectionStrings.DbzXXX").GetAwaiter().GetResult();
            connectionString = connectionString.Replace("XXX", clienteId.ToString().PadLeft(3, '0'));

            using (var db = new SqlConnection(connectionString))
            {
                var existentData = await db.GetAllAsync<NegocioConfig>();
                if (existentData.Count() == 0)
                    await db.InsertAsync(config);
                else
                    await db.UpdateAsync(config);
            }
        }

        public async Task<NegocioConfig> GetConfigAsync(int clienteId)
        {
            NegocioConfig result = null;

            var connectionString = _configurationService.GetValueAsync("ConnectionStrings.DbzXXX").GetAwaiter().GetResult();
            connectionString = connectionString.Replace("XXX", clienteId.ToString().PadLeft(3, '0'));

            using (var db = new SqlConnection(connectionString))
            {
                var existentData = await db.GetAllAsync<NegocioConfig>();
                result = existentData.FirstOrDefault();
            }

            return result;
        }
    }
}
