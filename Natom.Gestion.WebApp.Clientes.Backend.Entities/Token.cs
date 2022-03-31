using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities
{
    public class Token
    {
        [JsonProperty("uid")]
        public long UserId { get; set; }

        [JsonProperty("cid")]
        public int? ClientId { get; set; }

        [JsonProperty("sc")]
        public string Scope { get; set; }

        [JsonProperty("p")]
        public List<string> Permissions { get; set; }

        [JsonProperty("ct")]
        public long CreationTime { get; set; }

        [JsonProperty("et")]
        public long ExpirationTime { get; set; }

        [JsonProperty("dis")]
        public long Duration { get; set; }

        [JsonProperty("wec")]
        public bool WaitingEmailConfirmation { get; set; }
    }
}
