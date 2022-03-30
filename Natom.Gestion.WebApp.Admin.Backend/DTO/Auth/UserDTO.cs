using Natom.Gestion.Core.Biz.Entities.Models;
using Natom.Extensions.Auth.Entities.Models;
using Natom.Extensions.Auth.Entities.Results;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Auth
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

        [JsonProperty("cliente_encrypted_id")]
        public string ClienteEncryptedId { get; set; }

        [JsonProperty("cliente_nombre")]
        public string ClienteNombre { get; set; }

        [JsonProperty("me")]
        public bool Me { get; set; }

        public UserDTO From(Usuario entity)
        {
            EncryptedId = EncryptionService.Encrypt<Usuario>(entity.UsuarioId);
            FirstName = entity.Nombre;
            LastName = entity.Apellido;
            Email = entity.Email;
            PictureURL = "assets/img/user-photo.png";
            RegisteredAt = entity.FechaHoraAlta;
            Permisos = entity.Permisos?.Select(permiso => EncryptionService.Encrypt<Permiso>(permiso.PermisoId)).ToList();

            return this;
        }

        public UserDTO From(spUsuariosListByClienteAndScopeResult entity, int currentUsuarioId)
        {
            EncryptedId = EncryptionService.Encrypt<Usuario>(entity.UsuarioId);
            FirstName = entity.Nombre;
            LastName = entity.Apellido;
            Email = entity.Usuario;
            PictureURL = "assets/img/user-photo.png";
            RegisteredAt = entity.FechaHoraAlta;
            Status = entity.Estado;
            ClienteNombre = entity.ClienteRazonSocial;
            Me = entity.UsuarioId == currentUsuarioId;

            return this;
        }

        public Usuario ToModel(string scope)
        {
            var usuarioId = EncryptionService.Decrypt<int, Usuario>(this.EncryptedId);
            return new Usuario
            {
                UsuarioId = usuarioId,
                Scope = scope,
                Nombre = this.FirstName,
                Apellido = this.LastName,
                Email = this.Email,
                FechaHoraAlta = this.RegisteredAt,
                ClienteId = EncryptionService.Decrypt<int?, Cliente>(this.ClienteEncryptedId) ?? 0,
                Permisos = this.Permisos.Select(p => new UsuarioPermiso { UsuarioId = usuarioId, PermisoId = EncryptionService.Decrypt<string, Permiso>(p) }).ToList()
            };
        }
    }
}