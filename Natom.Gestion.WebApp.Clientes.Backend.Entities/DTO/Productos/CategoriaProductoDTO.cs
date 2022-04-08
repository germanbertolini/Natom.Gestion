using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Productos
{
    public class CategoriaProductoDTO
    {
        [JsonProperty("encrypted_id")]
        public string EncryptedId { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

		[JsonProperty("activo")]
		public bool Activo { get; set; }

		public CategoriaProductoDTO From(CategoriaProducto entity)
		{
			EncryptedId = EncryptionService.Encrypt<CategoriaProducto>(entity.CategoriaProductoId);
			Descripcion = entity.Descripcion;
			Activo = !entity.Eliminado;

			return this;
		}
	}
}
