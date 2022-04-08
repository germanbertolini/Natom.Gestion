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
    public class PrecioReajusteDTO
    {
		[JsonProperty("encrypted_id")]
		public string EncryptedId { get; set; }

		[JsonProperty("usuario")]
		public string Usuario { get; set; }

		[JsonProperty("esIncremento")]
		public bool EsIncremento { get; set; }

		[JsonProperty("esPorcentual")]
		public bool EsPorcentual { get; set; }

		[JsonProperty("valor")]
		public decimal Valor { get; set; }

		[JsonProperty("aplicoMarca_encrypted_id")]
		public string AplicoMarcaEncryptedId { get; set; }

		[JsonProperty("aplicoListaDePrecios_encrypted_id")]
		public string AplicoListaDePreciosEncryptedId { get; set; }

		public PrecioReajusteDTO From(HistoricoReajustePrecio entity)
		{
			EncryptedId = EncryptionService.Encrypt<HistoricoReajustePrecio>(entity.HistoricoReajustePrecioId);
			Usuario = entity.Usuario?.Nombre;
			EsIncremento = entity.EsIncremento;
			EsPorcentual = entity.EsPorcentual;
			Valor = entity.Valor;
			AplicoMarcaEncryptedId = EncryptionService.Encrypt<Marca>(entity.AplicoMarcaId);
			AplicoListaDePreciosEncryptedId = EncryptionService.Encrypt<ListaDePrecios>(entity.AplicoListaDePreciosId);

			return this;
		}
	}
}
