using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO
{
    public class ApiResultDTO
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class ApiResultDTO<TData> : ApiResultDTO
    {
        [JsonProperty("data")]
        public TData Data { get; set; }
    }
}
