using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Stock
{
    public class MovimientoStockDTO
    {
        [JsonProperty("deposito_encrypted_id")]
        public string DepositoEncryptedId { get; set; }

        [JsonProperty("producto_encrypted_id")]
        public string ProductoEncryptedId { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        [JsonProperty("cantidad")]
        public decimal Cantidad { get; set; }

        [JsonProperty("usuario_encrypted_id")]
        public string UsuarioEncryptedId { get; set; }

        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }

        [JsonProperty("esCompra")]
        public bool EsCompra { get; set; }

        [JsonProperty("proveedor_encrypted_id")]
        public string ProveedorEncryptedId { get; set; }

        [JsonProperty("costoUnitario")]
        public decimal? CostoUnitario { get; set; }
    }
}
