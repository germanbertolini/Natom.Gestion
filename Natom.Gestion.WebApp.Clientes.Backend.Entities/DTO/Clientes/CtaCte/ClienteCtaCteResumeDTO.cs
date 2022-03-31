using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Clientes.CtaCte
{
    public class ClienteCtaCteResumeDTO
    {
        [JsonProperty("monto")]
        public decimal Monto { get; set; }

        [JsonProperty("disponible")]
        public decimal Disponible { get; set; }

        [JsonProperty("deudor")]
        public decimal Deudor { get; set; }
    }
}
