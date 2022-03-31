using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Auth
{
    public class UserDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("picture_url")]
        public string PictureURL { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("registered_at")]
        public DateTime RegisteredAt { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("permisos")]
        public List<string> Permisos { get; set; }

        public UserDTO From(Usuario entity, string status = null)
        {
            EncryptedId = EncryptionService.Encrypt(entity.UsuarioId);
            FirstName = entity.Nombre;
            LastName = entity.Apellido;
            Email = entity.Email;
            PictureURL = "assets/img/user-photo.png";
            RegisteredAt = entity.FechaHoraAlta;
            Status = status;
            Permisos = entity.Permisos?.Select(permiso => EncryptionService.Encrypt(permiso.PermisoId)).ToList();

            return this;
        }
    }
}
