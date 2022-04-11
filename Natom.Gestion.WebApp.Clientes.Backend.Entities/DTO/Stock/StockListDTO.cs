using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Stock
{
    public class StockListDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("fechaHora")]
        public DateTime FechaHora { get; set; }

        [JsonProperty("fechaHoraControlado")]
        public DateTime? FechaHoraControlado { get; set; }

        [JsonProperty("deposito")]
        public string Deposito { get; set; }

        [JsonProperty("producto")]
        public string Producto { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        [JsonProperty("movido")]
        public decimal? Movido { get; set; }

        [JsonProperty("reservado")]
        public decimal? Reservado { get; set; }

        [JsonProperty("stock")]
        public decimal Stock { get; set; }

        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }

        public StockListDTO From(spMovimientosStockListResult entity)
        {
            EncryptedId = EncryptionService.Encrypt<MovimientoStock>(entity.MovimientoStockId);
            FechaHora = entity.FechaHora;
            Deposito = entity.Deposito;
            Producto = entity.Producto;
            Tipo = entity.Tipo.Equals("I") ? "Ingreso" : "Egreso";
            Movido = entity.Movido;
            Reservado = entity.Reservado;
            Stock = entity.Stock;
            Observaciones = entity.Observaciones;
            FechaHoraControlado = entity.FechaHoraControlado;

            return this;
        }
    }
}
