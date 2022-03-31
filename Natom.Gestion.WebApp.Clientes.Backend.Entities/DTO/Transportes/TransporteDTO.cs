﻿using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Transportes
{
    public class TransporteDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        public TransporteDTO From(Transporte entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.TransporteId);
            Descripcion = entity.Descripcion;
            Activo = entity.Activo;

            return this;
        }
    }
}