using Natom.Gestion.WebApp.Admin.Backend.Model.Results;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Synchronizers
{
    public class SyncDTO
    {
        [JsonProperty("encrypted_instance_id")]
        public string EncryptedInstanceId { get; set; }

        [JsonProperty("installation_alias")]
        public string InstallationAlias { get; set; }

		[JsonProperty("installed_by")]
		public string InstalledBy { get; set; }

		[JsonProperty("installed_at")]
		public DateTime InstalledAt { get; set; }

		[JsonProperty("activated_at")]
		public DateTime? ActivatedAt { get; set; }

		[JsonProperty("activo")]
		public bool Activo { get; set; }

		[JsonProperty("last_sync_at")]
		public DateTime? LastSyncAt { get; set; }

		[JsonProperty("devices_count")]
		public int DevicesCount { get; set; }


		public SyncDTO From(spSynchronizersListByClienteResult model)
        {
			this.EncryptedInstanceId = EncryptionService.Encrypt(model.InstanceId);
			this.InstallationAlias = model.InstallationAlias;
			this.InstalledBy = model.InstallerName;
			this.InstalledAt = model.InstalledAt;
			this.ActivatedAt = model.ActivatedAt;
			this.Activo = !model.Removed;
			this.LastSyncAt = model.LastSyncAt;
			this.DevicesCount = model.DevicesCount;

			return this;
		}
	}
}
