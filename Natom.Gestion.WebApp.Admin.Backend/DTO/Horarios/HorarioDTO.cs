using Natom.Gestion.Core.Biz.Entities.Models;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Horarios
{
    public class HorarioDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("encrypted_place_id")]
        public string EncryptedPlaceId { get; set; }

        [JsonProperty("configuro_fecha_hora")]
        public DateTime ConfiguroFechaHora { get; set; }

        [JsonProperty("ingreso_tolerancia_mins")]
        public int IngresoToleranciaMins { get; set; }

        [JsonProperty("egreso_tolerancia_mins")]
        public int EgresoToleranciaMins { get; set; }

        [JsonProperty("almuerzo_horario_desde")]
        public string AlmuerzoHorarioDesde { get; set; }

        [JsonProperty("almuerzo_horario_hasta")]
        public string AlmuerzoHorarioHasta { get; set; }

        [JsonProperty("almuerzo_tiempo_limite_mins")]
        public int AlmuerzoTiempoLimiteMins { get; set; }

        [JsonProperty("aplica_desde")]
        public DateTime AplicaDesde { get; set; }

        [JsonProperty("aplica_hasta")]
        public DateTime? AplicaHasta { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public HorarioDTO From(ConfigTolerancia entity)
        {
            EncryptedId = EncryptionService.Encrypt<ConfigTolerancia>(entity.ConfigToleranciaId);
            EncryptedPlaceId = EncryptionService.Encrypt<Place>(entity.PlaceId);

            IngresoToleranciaMins = entity.IngresoToleranciaMins;
            EgresoToleranciaMins = entity.EgresoToleranciaMins;
            AlmuerzoHorarioDesde = entity.AlmuerzoHorarioDesde.ToString(@"hh\:mm");
            AlmuerzoHorarioHasta = entity.AlmuerzoHorarioHasta.ToString(@"hh\:mm");
            AlmuerzoTiempoLimiteMins = entity.AlmuerzoTiempoLimiteMins;

            ConfiguroFechaHora = entity.ConfiguroFechaHora;

            AplicaDesde = entity.AplicaDesde;
            AplicaHasta = entity.AplicaHasta;

            Status = entity.GetStatus();


            return this;
        }

        public ConfigTolerancia ToModel(int clienteId, HorarioDTO dto)
        {
            return new ConfigTolerancia
            {
                ConfigToleranciaId = EncryptionService.Decrypt<int, ConfigTolerancia>(dto.EncryptedId),
                PlaceId = EncryptionService.Decrypt<int, Place>(dto.EncryptedPlaceId),
                ClienteId = clienteId,
                IngresoToleranciaMins = dto.IngresoToleranciaMins,
                EgresoToleranciaMins = dto.EgresoToleranciaMins,
                AlmuerzoHorarioDesde = TimeSpan.Parse($"{dto.AlmuerzoHorarioDesde}:00"),
                AlmuerzoHorarioHasta = TimeSpan.Parse($"{dto.AlmuerzoHorarioHasta}:00"),
                AlmuerzoTiempoLimiteMins = dto.AlmuerzoTiempoLimiteMins,
                ConfiguroFechaHora = dto.ConfiguroFechaHora,
                AplicaDesde = dto.AplicaDesde,
                AplicaHasta = dto.AplicaHasta
            };
        }
    }
}
