using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Precios
{
    public class PrecioListDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("producto")]
        public string Producto { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        [JsonProperty("listaDePrecios")]
        public string ListaDePrecios { get; set; }

        [JsonProperty("aplicaDesdeFechaHora")]
        public DateTime AplicaDesdeFechaHora { get; set; }

        [JsonProperty("aplicaDesdeDias")]
        public int AplicaDesdeDias { get; set; }

        [JsonProperty("esPorcentual")]
        public bool EsPorcentual { get; set; }

        public PrecioListDTO From(spPreciosListResult entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.ProductoPrecioId);
            Producto = entity.ProductoDescripcion;
            Precio = entity.Precio;
            ListaDePrecios = entity.ListaDePrecioDescripcion;
            AplicaDesdeFechaHora = entity.AplicaDesdeFechaHora;
            AplicaDesdeDias = (int)(DateTime.Now.Date - entity.AplicaDesdeFechaHora.Date).TotalDays;
            EsPorcentual = entity.ListaDePreciosEsPorcentual && !entity.ProductoPrecioId.HasValue;

            return this;
        }
    }
}
