using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Results
{
    public class spSynchronizerSelectSyncTimesResult
    {
        public string InstanceId { get; set; }
        public string InstallerName { get; set; }
		public DateTime? LastSyncAt { get; set; }
        public DateTime? NextSyncAt { get; set; }
        public long EachMiliseconds { get; set; }
        public long NextOnMiliseconds { get; set; }
    }
}
