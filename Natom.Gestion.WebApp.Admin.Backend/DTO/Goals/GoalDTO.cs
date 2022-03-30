using Natom.Gestion.Core.Biz.Entities.Models;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Goals
{
    public class GoalDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("place_encrypted_id")]
        public string PlaceEncryptedId { get; set; }

        [JsonProperty("place_name")]
        public string PlaceName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public GoalDTO From(Goal entity)
        {
            EncryptedId = EncryptionService.Encrypt<Goal>(entity.GoalId);
            Name = entity.Name;
            Address = entity.Address;
            Activo = !entity.RemovedAt.HasValue;
            PlaceEncryptedId = EncryptionService.Encrypt<Place>(entity.PlaceId);
            PlaceName = entity.Place?.Name;

            return this;
        }

        public Goal ToModel()
        {
            return new Goal
            {
                GoalId = EncryptionService.Decrypt<int, Goal>(EncryptedId),
                Name = Name,
                Lat = null,
                Lng = null,
                Address = Address,
                PlaceId = EncryptionService.Decrypt<int, Place>(PlaceEncryptedId)
            };
        }
    }
}
