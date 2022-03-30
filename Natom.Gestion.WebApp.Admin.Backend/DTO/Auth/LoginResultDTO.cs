using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Auth
{
    public class LoginResultDTO
    {
        [JsonProperty("user")]
        public UserDTO User { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("permissions")]
        public List<string> Permissions { get; set; }
    }
}
