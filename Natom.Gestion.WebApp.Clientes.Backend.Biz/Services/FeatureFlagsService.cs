using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Services
{
    public class FeatureFlagsService
    {
        public FeatureFlagsDTO FeatureFlags { get; set; }

        public FeatureFlagsService()
        {
            string jsonContent = File.ReadAllText("featureflags.json");
            this.FeatureFlags = JsonConvert.DeserializeObject<FeatureFlagsDTO>(jsonContent);
        }
    }
}
