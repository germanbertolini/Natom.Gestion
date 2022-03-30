using Natom.Gestion.Core.Biz.Entities.Models;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Zonas
{
    public class ZonaDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public ZonaDTO From(Zona entity)
        {
            EncryptedId = EncryptionService.Encrypt<Zona>(entity.ZonaId);
            Descripcion = entity.Descripcion;
            Activo = entity.Activo;

            return this;
        }

        public Zona ToModel()
        {
            return new Zona
            {
                ZonaId = EncryptionService.Decrypt<int, Zona>(EncryptedId),
                Descripcion = Descripcion,
                Activo = Activo
            };
        }
    }
}
