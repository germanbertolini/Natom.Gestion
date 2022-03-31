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
    public class ListaDePreciosDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonProperty("esPorcentual")]
        public bool EsPorcentual { get; set; }


        public ListaDePreciosDTO From(ListaDePrecios entity)
        {
            EncryptedId = EncryptionService.Encrypt(entity.ListaDePreciosId);
            Descripcion = entity.Descripcion;
            Activo = entity.Activo;
            EsPorcentual = entity.EsPorcentual;

            return this;
        }
    }
}
