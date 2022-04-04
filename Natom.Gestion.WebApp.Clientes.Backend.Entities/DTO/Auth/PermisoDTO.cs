using Natom.Extensions.Auth.Entities.Models;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Auth
{
    public class PermisoDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        public PermisoDTO From(Permiso entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.PermisoId);
            Descripcion = entity.Descripcion;

            return this;
        }
    }
}
