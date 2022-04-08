using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Precios
{
    public class PrecioDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("producto")]
        public string Producto { get; set; }

        [JsonProperty("producto_encrypted_id")]
        public string ProductoEncryptedId { get; set; }

        [JsonProperty("listaDePrecios_encrypted_id")]
        public string ListaDePreciosEncryptedId { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        public PrecioDTO From(ProductoPrecio entity)
        {
            EncryptedId = EncryptionService.Encrypt<ProductoPrecio>(entity.ProductoPrecioId);
            Producto = entity.Producto != null ? $"({entity.Producto?.Codigo}) {entity.Producto?.DescripcionCorta}" : "";
            ProductoEncryptedId = EncryptionService.Encrypt<Producto>(entity.ProductoId);
            ListaDePreciosEncryptedId = EncryptionService.Encrypt<ListaDePrecios>(entity.ListaDePreciosId);
            Precio = entity.Precio;

            return this;
        }
    }
}
