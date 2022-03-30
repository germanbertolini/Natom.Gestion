using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Synchronizers
{
    public class PendingToActivateDTO
    {
        public string InstanceId { get; set; }
        public string InstallationAlias { get; set; }
        public string InstallerName { get; set; }
        public string ClientName { get; set; }
        public string ClientCUIT { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public int? ActivatedToClientId { get; set; }
    }
}
