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
    public class VentaListDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }

        [JsonProperty("numeroVenta")]
        public string NumeroVenta { get; set; }

        [JsonProperty("remitos")]
        public List<string> Remitos { get; set; }
        
        [JsonProperty("pedidos")]
        public List<string> Pedidos { get; set; }

        [JsonProperty("factura")]
        public string Factura { get; set; }

        [JsonProperty("cliente")]
        public string Cliente { get; set; }

        [JsonProperty("fechaHora")]
        public DateTime FechaHora { get; set; }

        [JsonProperty("usuario")]
        public string Usuario { get; set; }

        [JsonProperty("medio_de_pago")]
        public string MedioDePago { get; set; }

        [JsonProperty("peso_total_gramos")]
        public int PesoTotalGramos { get; set; }

        [JsonProperty("anulado")]
        public bool Anulado { get; set; }

        public VentaListDTO From(Venta entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.VentaId);
            Numero = entity.NumeroVenta.ToString().PadLeft(8, '0');
            FechaHora = entity.FechaHoraVenta;
            NumeroVenta = entity.NumeroVenta.ToString().PadLeft(8, '0');
            var numerosRemitos = entity.Detalle.Select(d => d.OrdenDePedido?.NumeroRemito).Where(d => !string.IsNullOrEmpty(d)).ToList();
            Remitos = numerosRemitos.Where(r => !string.IsNullOrEmpty(r)).GroupBy(k => k, (k, v) => k).Select(r => $"RTO {r}").ToList();
            Pedidos = entity.Detalle.Select(d => d.OrdenDePedido?.NumeroPedido.ToString().PadLeft(8, '0')).GroupBy(k => k, (k, v) => k).Where(r => !string.IsNullOrEmpty(r)).ToList();
            Factura = string.IsNullOrEmpty(entity.NumeroFactura) ? null : entity.TipoFactura + " " + entity.NumeroFactura;
            Cliente = entity.Cliente.EsEmpresa ? entity.Cliente.RazonSocial : $"{entity.Cliente.Nombre} {entity.Cliente.Apellido}";

            if (Cliente != null && string.IsNullOrEmpty(Cliente.Trim()))
                Cliente = $"{entity?.Cliente?.Domicilio}, {entity?.Cliente?.Localidad}";

            Usuario = entity.Usuario?.Nombre ?? "Admin";
            PesoTotalGramos = entity.PesoTotalEnGramos;
            Anulado = !entity.Activo;
            MedioDePago = entity.MedioDePago;

            return this;
        }
    }
}
