using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Cajas
{
    public class MovimientoCajaFuerteDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("fechaHora")]
        public DateTime FechaHora { get; set; }

        [JsonProperty("usuarioNombre")]
        public string UsuarioNombre { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        [JsonProperty("importe")]
        public decimal Importe { get; set; }

        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }

        [JsonProperty("esCheque")]
        public bool EsCheque { get; set; }

        [JsonProperty("esCtaCte")]
        public bool EsCtaCte { get; set; }

        [JsonProperty("medio_de_pago")]
        public string MedioDePago { get; set; }

        [JsonProperty("pago_referencia")]
        public string PagoReferencia { get; set; }

        [JsonProperty("proveedor_encrypted_id")]
        public string ProveedorEncryptedId { get; set; }

        public MovimientoCajaFuerteDTO From(MovimientoCajaFuerte entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.MovimientoCajaFuerteId);
            FechaHora = entity.FechaHora;
            UsuarioNombre = entity.Usuario?.Nombre ?? "Admin";
            Tipo = entity.Tipo.Equals("C") ? "Ingreso" : "Egreso";
            Importe = entity.Importe;
            Observaciones = entity.Observaciones;
            EsCheque = entity.EsCheque;
            MedioDePago = entity.MedioDePago;
            PagoReferencia = entity.Referencia;

            return this;
        }
    }
}
