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
    public class PrecioReajusteListDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("usuario")]
        public string Usuario { get; set; }

        [JsonProperty("tipoReajuste")]
        public string TipoReajuste { get; set; }

        [JsonProperty("esPorcentual")]
        public bool EsPorcentual { get; set; }

        [JsonProperty("valorReajuste")]
        public decimal ValorReajuste { get; set; }

        [JsonProperty("aplicoMarca")]
        public string AplicoMarca { get; set; }

        [JsonProperty("aplicoListaDePrecios")]
        public string AplicoListaDePrecios { get; set; }

        [JsonProperty("aplicaDesdeFechaHora")]
        public DateTime AplicaDesdeFechaHora { get; set; }

        [JsonProperty("anulado")]
        public bool Anulado { get; set; }

        public PrecioReajusteListDTO From(HistoricoReajustePrecio entity)
        {
            EncryptedId = EncryptionService.Encrypt<HistoricoReajustePrecio>(entity.HistoricoReajustePrecioId);
            Usuario = entity.Usuario?.Nombre ?? "Admin";
            TipoReajuste = $"{(entity.EsIncremento ? "Aumento" : "Decremento")} - {(entity.EsPorcentual ? "Porcentaje" : "Monto")}";
            EsPorcentual = entity.EsPorcentual;
            ValorReajuste = entity.Valor;
            AplicoMarca = entity.AplicoMarca?.Descripcion;
            AplicoListaDePrecios = entity.AplicoListaDePrecios?.Descripcion ?? "Todas";
            AplicaDesdeFechaHora = entity.AplicaDesdeFechaHora;
            Anulado = entity.FechaHoraBaja.HasValue;

            return this;
        }
    }
}
