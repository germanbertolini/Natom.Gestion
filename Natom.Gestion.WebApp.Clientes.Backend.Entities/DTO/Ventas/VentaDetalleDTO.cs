using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Ventas
{
    public class VentaDetalleDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("venta_encrypted_id")]
        public string VentaEncryptedId { get; set; }

        [JsonProperty("producto_encrypted_id")]
        public string ProductoEncryptedId { get; set; }

        [JsonProperty("producto_descripcion")]
        public string ProductoDescripcion { get; set; }

        [JsonProperty("producto_peso_gramos")]
        public int? ProductoPesoGramos { get; set; }

        [JsonProperty("pedido_numero")]
        public string PedidoNumero { get; set; }

        [JsonProperty("cantidad")]
        public decimal Cantidad { get; set; }

        [JsonProperty("deposito_encrypted_id")]
        public string DepositoEncryptedId { get; set; }

        [JsonProperty("deposito_descripcion")]
        public string DepositoDescripcion { get; set; }

        [JsonProperty("precio_lista_encrypted_id")]
        public string PrecioListaEncryptedId { get; set; }

        [JsonProperty("precio_descripcion")]
        public string PrecioDescripcion { get; set; }

        [JsonProperty("numero_remito")]
        public string NumeroRemito { get; set; }

        [JsonProperty("precio")]
        public decimal? Precio { get; set; }

        [JsonProperty("pedido_encrypted_id")]
        public string OrdenDePedidoEncryptedId { get; set; }

        [JsonProperty("pedido_detalle_encrypted_id")]
        public string OrdenDePedidoDetalleEncryptedId { get; set; }


        public VentaDetalleDTO From(VentaDetalle entity)
        {
            EncryptedId = EncryptionService.Encrypt<VentaDetalle>(entity.VentaDetalleId);
            VentaEncryptedId = EncryptionService.Encrypt<Venta>(entity.VentaId);
            ProductoEncryptedId = EncryptionService.Encrypt<Producto>(entity.ProductoId);
            ProductoDescripcion = entity.Producto?.DescripcionCorta;
            ProductoPesoGramos = entity.PesoUnitarioEnGramos;
            Cantidad = entity.Cantidad;
            DepositoEncryptedId = EncryptionService.Encrypt<Deposito>(entity.DepositoId);
            DepositoDescripcion = entity.Deposito?.Descripcion;
            PedidoNumero = entity.OrdenDePedido?.NumeroPedido.ToString().PadLeft(8, '0');
            PrecioListaEncryptedId = EncryptionService.Encrypt<ListaDePrecios>(entity.ListaDePreciosId);
            PrecioDescripcion = entity.ListaDePrecios?.Descripcion ?? "";
            NumeroRemito = entity.NumeroRemito;
            Precio = entity.Precio;
            OrdenDePedidoEncryptedId = EncryptionService.Encrypt<OrdenDePedido>(entity.OrdenDePedidoId);
            OrdenDePedidoDetalleEncryptedId = EncryptionService.Encrypt<OrdenDePedidoDetalle>(entity.OrdenDePedidoDetalleId);
            return this;
        }
    }
}