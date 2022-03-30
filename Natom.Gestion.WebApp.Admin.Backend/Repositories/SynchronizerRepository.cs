using Dapper;
using Microsoft.Data.SqlClient;
using Natom.Extensions.Auth.Entities.Models;
using Natom.Extensions.Configuration.Services;
using Natom.Gestion.WebApp.Admin.Backend.Model.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Repositories
{
    public class SynchronizerRepository
    {
        private readonly string _connectionString;

        public SynchronizerRepository(ConfigurationService configuration)
        {
            _connectionString = configuration.GetValueAsync("ConnectionStrings.DbSecurity").GetAwaiter().GetResult();
        }

        public async Task<List<spSynchronizersListByClienteResult>> ListByClienteAsync(int clienteId, string search, int skip, int take)
        {
            List<spSynchronizersListByClienteResult> syncs = null;
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizers_list_by_cliente] @ClienteId, @Search, @Skip, @Take";
                var _params = new { ClienteId = clienteId, Search = search, Skip = skip, Take = take };
                syncs = (await db.QueryAsync<spSynchronizersListByClienteResult>(sql, _params)).ToList();
            }
            return syncs;
        }

        public async Task<List<spDeviceListBySyncIdResult>> ListDevicesBySyncAsync(string syncInstanceId, string search, int skip, int take)
        {
            List<spDeviceListBySyncIdResult> syncs = null;
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_device_list_by_syncid] @SyncInstanceId, @Search, @Skip, @Take";
                var _params = new { SyncInstanceId = syncInstanceId, Search = search, Skip = skip, Take = take };
                syncs = (await db.QueryAsync<spDeviceListBySyncIdResult>(sql, _params)).ToList();
            }
            return syncs;
        }

        public async Task<List<Token>> BajaAsync(string syncInstanceId)
        {
            List<Token> tokens = null;
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizer_baja_by_id] @SyncInstanceId";
                var _params = new { SyncInstanceId = syncInstanceId };
                tokens = (await db.QueryAsync<Token>(sql, _params)).ToList();
            }
            return tokens;
        }

        public async Task ActivarYEnlazarAsync(string instanceId, int clienteId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizer_alta_y_enlazar] @InstanceId, @ClienteId";
                var _params = new { InstanceId = instanceId, ClienteId = clienteId };
                await db.ExecuteAsync(sql, _params);
            }
        }
    }
}
