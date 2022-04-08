using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Pedidos
{
    public class PedidoDetalleDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("pedido_encrypted_id")]
        public string PedidoEncryptedId { get; set; }

        [JsonProperty("producto_encrypted_id")]
        public string ProductoEncryptedId { get; set; }

        [JsonProperty("producto_descripcion")]
        public string ProductoDescripcion { get; set; }

        [JsonProperty("producto_peso_gramos")]
        public int ProductoPesoGramos { get; set; }

        [JsonProperty("cantidad")]
        public int Cantidad { get; set; }

        [JsonProperty("entregado")]
        public int? Entregado { get; set; }

        [JsonProperty("deposito_encrypted_id")]
        public string DepositoEncryptedId { get; set; }

        [JsonProperty("deposito_descripcion")]
        public string DepositoDescripcion { get; set; }

        [JsonProperty("precio_lista_encrypted_id")]
        public string PrecioListaEncryptedId { get; set; }

        [JsonProperty("precio_descripcion")]
        public string PrecioDescripcion { get; set; }

        [JsonProperty("precio")]
        public decimal? Precio { get; set; }


        public PedidoDetalleDTO From(OrdenDePedidoDetalle entity)
        {
            EncryptedId = EncryptionService.Encrypt<OrdenDePedidoDetalle>(entity.OrdenDePedidoDetalleId);
            PedidoEncryptedId = EncryptionService.Encrypt<OrdenDePedido>(entity.OrdenDePedidoId);
            ProductoEncryptedId = EncryptionService.Encrypt<Producto>(entity.ProductoId);
            ProductoDescripcion = entity.Producto.DescripcionCorta;
            ProductoPesoGramos = entity.PesoUnitarioEnGramos;
            Cantidad = entity.Cantidad;
            Entregado = entity.CantidadEntregada;
            DepositoEncryptedId = EncryptionService.Encrypt<Deposito>(entity.DepositoId);
            DepositoDescripcion = entity.Deposito?.Descripcion;
            PrecioListaEncryptedId = EncryptionService.Encrypt<ListaDePrecios>(entity.ListaDePreciosId);
            PrecioDescripcion = entity.ListaDePrecios?.Descripcion ?? "< a definir en Venta >";
            Precio = entity.Precio;

            return this;
        }
    }
}