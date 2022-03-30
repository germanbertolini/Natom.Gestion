using Dapper;
using Microsoft.Data.SqlClient;
using Natom.AccessMonitor.Core.Biz.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Managers
{
    public class SyncsManager : BaseManager
    {
        public SyncsManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public async Task<List<spDeviceListByClientIdResult>> ListDevicesByClienteAsync(string search, int skip, int take, int clienteId)
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbSecurity");

            List<spDeviceListByClientIdResult> devices = null;
            using (var db = new SqlConnection(connectionString))
            {
                var sql = "EXEC [dbo].[sp_device_list_by_clientid] @ClienteId, @Search, @Skip, @Take";
                var _params = new { ClienteId = clienteId, Search = search, Skip = skip, Take = take };
                devices = (await db.QueryAsync<spDeviceListByClientIdResult>(sql, _params)).ToList();
            }
            return devices;
        }

        public async Task AssignDeviceToGoalAsync(int deviceId, int goalId)
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbSecurity");

            using (var db = new SqlConnection(connectionString))
            {
                var sql = "EXEC [dbo].[sp_device_assign_to_goal] @Id, @GoalId";
                var _params = new { Id = deviceId, GoalId = goalId };
                await db.ExecuteAsync(sql, _params);
            }
        }

        public async Task<List<spDeviceListUnassignedByClientIdResult>> GetUnassignedDevicesByClientIdAsync(int clienteId)
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbSecurity");
            var unassigned = new List<spDeviceListUnassignedByClientIdResult>();
            using (var db = new SqlConnection(connectionString))
            {
                var sql = "EXEC [dbo].[sp_device_list_unassigned_by_clientid] @ClientId";
                var _params = new { ClientId = clienteId };
                unassigned = (await db.QueryAsync<spDeviceListUnassignedByClientIdResult>(sql, _params)).ToList();
            }
            return unassigned;
        }

        public async Task<spSynchronizerSelectConfigByIdResult> GetSynchronizerConfigByIdAsync(string instanceId)
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbSecurity");

            spSynchronizerSelectConfigByIdResult config = null;
            using (var db = new SqlConnection(connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizer_select_config_by_id] @InstanceId";
                var _params = new { InstanceId = instanceId };
                config = (await db.QueryAsync<spSynchronizerSelectConfigByIdResult>(sql, _params)).First();
            }
            return config;
        }

        public async Task<List<spSynchronizerSelectSyncTimesResult>> GetSynchronizerTimesByClienteAsync(int clienteId)
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbSecurity");

            var times = new List<spSynchronizerSelectSyncTimesResult>();
            using (var db = new SqlConnection(connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizer_select_sync_times] @ClienteId";
                var _params = new { ClienteId = clienteId };
                times = (await db.QueryAsync<spSynchronizerSelectSyncTimesResult>(sql, _params)).ToList();
            }
            return times;
        }

        public async Task SaveSynchronizerConfigByIdAsync(string instanceId, int? intervalMinsFromDevice, int? intervalMinsToServer)
        {
            var connectionString = await _configuration.GetValueAsync("ConnectionStrings.DbSecurity");

            using (var db = new SqlConnection(connectionString))
            {
                var sql = "EXEC [dbo].[sp_synchronizer_save_config_by_id] @InstanceId, @IntervalMinsFromDevice, @IntervalMinsToServer";
                var _params = new { InstanceId = instanceId, IntervalMinsFromDevice = intervalMinsFromDevice, IntervalMinsToServer = intervalMinsToServer };
                await db.ExecuteAsync(sql, _params);
            }
        }
    }
}
