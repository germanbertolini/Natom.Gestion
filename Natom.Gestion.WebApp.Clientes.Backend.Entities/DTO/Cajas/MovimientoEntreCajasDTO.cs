using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Cajas
{
    public class MovimientoEntreCajasDTO
    {
        [JsonProperty("origen")]
        public string Origen { get; set; }

        [JsonProperty("destino")]
        public string Destino { get; set; }

        [JsonProperty("importe")]
        public decimal Importe { get; set; }

        [JsonProperty("usuarioNombre")]
        public string UsuarioNombre { get; set; }

        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }

        [JsonProperty("esCheque")]
        public bool EsCheque { get; set; }
    }
}
