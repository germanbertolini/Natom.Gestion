using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Results
{
    public class spDeviceListByClientIdResult
    {
		public int Id { get; set; }
		public string InstanceId { get; set; }
		public string DeviceId { get; set; }
		public string DeviceName { get; set; }
		public string DeviceIP { get; set; }
		public string Goal { get; set; }
		public decimal? Lat { get; set; }
		public decimal? Lng { get; set; }
		public string Place { get; set; }
		public DateTime AddedAt { get; set; }
		public string SerialNumber { get; set; }
		public string Model { get; set; }
		public string Brand { get; set; }
		public string SyncName { get; set; }

		public DateTime? LastSyncAt { get; set; }
		public DateTime? NextSyncAt { get; set; }

		public bool IsOnline { get; set; }

		public int TotalFiltrados { get; set; }
        public int TotalRegistros { get; set; }
        
    }
}
