using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Results
{
    public class spSynchronizerSelectConfigByIdResult
    {
		public string InstanceId { get; set; }
		public int? SyncToServerMinutes { get; set; }
		public int? SyncFromDevicesMinutes { get; set; }
		public DateTime? LastSyncAt { get; set; }
		public DateTime? NextSyncAt { get; set; }
	}
}
