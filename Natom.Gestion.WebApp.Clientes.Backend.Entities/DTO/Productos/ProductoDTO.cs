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
    public class ProductoDTO
    {
		[JsonProperty("encrypted_id")]
		public string EncryptedId { get; set; }

		[JsonProperty("marca_encrypted_id")]
		public string MarcaEncryptedId { get; set; }

		[JsonProperty("codigo")]
		public string Codigo { get; set; }

		[JsonProperty("descripcionCorta")]
		public string DescripcionCorta { get; set; }

		[JsonProperty("descripcionLarga")]
		public string DescripcionLarga { get; set; }

		[JsonProperty("unidadPeso_encrypted_id")]
		public string UnidadPesoEncryptedId { get; set; }

		[JsonProperty("pesoUnitario")]
		public decimal PesoUnitario { get; set; }

		[JsonProperty("categoria_encrypted_id")]
		public string CategoriaEncryptedId { get; set; }

		[JsonProperty("mueveStock")]
		public bool MueveStock { get; set; }

		[JsonProperty("costo_unitario")]
        public decimal? CostoUnitario { get; set; }

        public ProductoDTO From(Producto entity)
		{
			EncryptedId = EncryptionService.Encrypt<Producto>(entity.ProductoId);
			MarcaEncryptedId = EncryptionService.Encrypt<Marca>(entity.MarcaId);
			Codigo = entity.Codigo;
			DescripcionCorta = entity.DescripcionCorta;
			DescripcionLarga = entity.DescripcionLarga;
			UnidadPesoEncryptedId = EncryptionService.Encrypt<UnidadPeso>(entity.UnidadPesoId);
			CategoriaEncryptedId = EncryptionService.Encrypt<CategoriaProducto>(entity.CategoriaProductoId);
			PesoUnitario = entity.PesoUnitario;
			CostoUnitario = entity.CostoUnitario;
			MueveStock = entity.MueveStock;

			return this;
		}
	}
}
