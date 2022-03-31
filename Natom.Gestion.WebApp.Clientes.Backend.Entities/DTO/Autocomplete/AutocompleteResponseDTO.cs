using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Autocomplete
{
    public class AutocompleteResponseDTO<TResult>
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("results")]
        public List<TResult> Results { get; set; }
    }
}
