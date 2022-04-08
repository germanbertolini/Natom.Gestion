using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Ventas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Ventas
{
    public class VentaDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("fechaHora")]
        public DateTime FechaHora { get; set; }

        [JsonProperty("cliente_encrypted_id")]
        public string ClienteEncryptedId { get; set; }

        [JsonProperty("usuario")]
        public string Usuario { get; set; }

        [JsonProperty("tipo_factura")]
        public string TipoFactura { get; set; }

        [JsonProperty("numero_factura")]
        public string NumeroFactura { get; set; }

        [JsonProperty("medio_de_pago")]
        public string MedioDePago { get; set; }

        [JsonProperty("pago_referencia")]
        public string PagoReferencia { get; set; }

        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }

        [JsonProperty("detalle")]
        public List<VentaDetalleDTO> Detalle { get; set; }

        [JsonProperty("pedidos")]
        public List<VentaDetalleDTO> Pedidos { get; set; }

        public VentaDTO From(Venta entity)
        {
            EncryptedId = EncryptionService.Encrypt<Venta>(entity.VentaId);
            Numero = entity.NumeroVenta.ToString().PadLeft(8, '0');
            FechaHora = entity.FechaHoraVenta;
            ClienteEncryptedId = EncryptionService.Encrypt<Cliente>(entity.ClienteId);
            Usuario = entity.Usuario?.Nombre ?? "Admin";
            TipoFactura = entity.TipoFactura;
            NumeroFactura = entity.NumeroFactura;
            Observaciones = entity.Observaciones;
            Detalle = entity.Detalle.Select(d => new VentaDetalleDTO().From(d)).ToList();
            MedioDePago = entity.MedioDePago;
            PagoReferencia = entity.PagoReferencia;

            return this;
        }
    }
}
