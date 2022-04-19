using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Negocio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO
{
    public class AppConfigDTO
    {
        [JsonProperty("feature_flags")]
        public FeatureFlagsDTO FeatureFlags { get; set; }

        [JsonProperty("negocio_config")]
        public NegocioConfigDTO NegocioConfig { get; set; }
    }
}
