using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Pedidos
{
    public class PedidoDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("fechaHora")]
        public DateTime FechaHora { get; set; }

        [JsonProperty("cliente_encrypted_id")]
        public string ClienteEncryptedId { get; set; }

        [JsonProperty("entrega_estimada_fecha")]
        public DateTime? EntregaEstimadaFecha { get; set; }

        [JsonProperty("entrega_estimada_rango_horario_encrypted_id")]
        public string EntregaEstimadaRangoHorarioEncryptedId { get; set; }

        [JsonProperty("entrega_observaciones")]
        public string EntregaObservaciones { get; set; }

        [JsonProperty("entrega_domicilio")]
        public string EntregaDomicilio { get; set; }

        [JsonProperty("entrega_entre_calles")]
        public string EntregaEntreCalles { get; set; }

        [JsonProperty("entrega_localidad")]
        public string EntregaLocalidad { get; set; }

        [JsonProperty("entrega_telefono1")]
        public string EntregaTelefono1 { get; set; }

        [JsonProperty("entrega_telefono2")]
        public string EntregaTelefono2 { get; set; }

        [JsonProperty("entregado")]
        public bool Entregado { get; set; }

        [JsonProperty("usuario")]
        public string Usuario { get; set; }

        [JsonProperty("numero_remito")]
        public string NumeroRemito { get; set; }

        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }

        [JsonProperty("medio_de_pago")]
        public string MedioDePago { get; set; }

        [JsonProperty("pago_referencia")]
        public string PagoReferencia { get; set; }

        [JsonProperty("retira_personalmente")]
        public bool RetiraPersonalmente { get; set; }

        [JsonProperty("detalle")]
        public List<PedidoDetalleDTO> Detalle { get; set; }

        public PedidoDTO From(OrdenDePedido entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.OrdenDePedidoId);
            Numero = entity.NumeroPedido.ToString().PadLeft(8, '0');
            FechaHora = entity.FechaHoraPedido;
            ClienteEncryptedId = EncryptionService.Encrypt(entity.ClienteId);
            EntregaEstimadaFecha = entity.EntregaEstimadaFecha;
            EntregaEstimadaRangoHorarioEncryptedId = EncryptionService.Encrypt(entity.EntregaEstimadaRangoHorarioId);
            EntregaObservaciones = entity.EntregaObservaciones;
            Usuario = entity.Usuario?.Nombre ?? "Admin";
            NumeroRemito = entity.NumeroRemito;
            Observaciones = entity.Observaciones;
            Detalle = entity.Detalle?.Select(d => new PedidoDetalleDTO().From(d)).ToList();
            EntregaDomicilio = entity.EntregaDomicilio;
            EntregaEntreCalles = entity.EntregaEntreCalles;
            EntregaLocalidad = entity.EntregaLocalidad;
            EntregaTelefono1 = entity.EntregaTelefono1;
            EntregaTelefono2 = entity.EntregaTelefono2;
            Entregado = entity.MarcoEntregaUsuarioId.HasValue;
            MedioDePago = entity.MedioDePago;
            PagoReferencia = entity.PagoReferencia;
            RetiraPersonalmente = entity.RetiraPersonalmente;

            return this;
        }
    }
}
