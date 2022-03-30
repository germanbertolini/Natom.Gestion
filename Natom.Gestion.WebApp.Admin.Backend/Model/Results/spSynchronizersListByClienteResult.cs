using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Model.Results
{
    public class spSynchronizersListByClienteResult
    {
		public string InstanceId { get; set; }
		public string InstallationAlias { get; set; }
		public string InstallerName { get; set; }
		public DateTime InstalledAt { get; set; }
		public DateTime? ActivatedAt { get; set; }
		public bool Removed { get; set; }
		public DateTime? LastSyncAt { get; set; }
		public int DevicesCount { get; set; }

		public int TotalFiltrados { get; set; }
		public int TotalRegistros { get; set; }
	}
}
