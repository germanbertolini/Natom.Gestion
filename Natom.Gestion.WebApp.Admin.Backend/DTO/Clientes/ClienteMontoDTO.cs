using Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Clientes
{
    public class ClienteMontoDTO
    {
		[JsonProperty("encrypted_id")]
		public string EncryptedId { get; set; }

		[JsonProperty("monto")]
		public decimal Monto { get; set; }

		[JsonProperty("desde")]
		public DateTime Desde { get; set; }

		public ClienteMontoDTO From(ClienteMonto entity)
		{
			EncryptedId = EncryptionService.Encrypt<ClienteMonto>(entity.ClienteId);
			Monto = entity.Monto;
			Desde = entity.Desde;

			return this;
		}

		public ClienteMonto ToModel()
		{
			return new ClienteMonto
			{
				ClienteMontoId = EncryptionService.Decrypt<int, ClienteMonto>(EncryptedId),
				Desde = Desde,
				Monto = Monto
			};
		}
	}
}
