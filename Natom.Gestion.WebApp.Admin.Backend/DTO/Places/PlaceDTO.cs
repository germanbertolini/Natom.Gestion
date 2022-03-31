using Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Places
{
    public class PlaceDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("pending_goals")]
        public bool PendingGoals { get; set; }

        [JsonProperty("pending_tolerancias")]
        public bool PendingTolerancias { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public PlaceDTO From(Place entity)
        {
            EncryptedId = EncryptionService.Encrypt<Place>(entity.PlaceId);
            Name = entity.Name;
            Address = entity.Address;
            Activo = !entity.RemovedAt.HasValue;
            PendingGoals = (entity.Goals?.Count(g => !g.RemovedAt.HasValue) ?? 0) == 0;
            PendingTolerancias = (entity.ConfigTolerancias?.Count(g => !g.AplicaHasta.HasValue) ?? 0) == 0;

            return this;
        }

        public Place ToModel(int clientId)
        {
            return new Place
            {
                PlaceId = EncryptionService.Decrypt<int, Place>(EncryptedId),
                Name = Name,
                Address = Address,
                Lat = null,
                Lng = null,
                ClientId = clientId
            };
        }
    }
}
