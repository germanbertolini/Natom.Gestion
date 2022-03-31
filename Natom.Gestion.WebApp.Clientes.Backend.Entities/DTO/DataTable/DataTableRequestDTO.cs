using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable
{
    public class DataTableRequestDTO
    {
        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("search")]
        public DataTableSearch Search { get; set; }

        [JsonProperty("order")]
        public List<DataTableSortOrder> Order { get; set; }
    }

    public class DataTableSearch
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class DataTableSortOrder
    {
        [JsonProperty("column")]
        public int ColumnIndex { get; set; }

        [JsonProperty("dir")]
        public string Direction { get; set; }
    }
}
