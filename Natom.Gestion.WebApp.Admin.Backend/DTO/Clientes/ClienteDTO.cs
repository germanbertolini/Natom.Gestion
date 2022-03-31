using Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models;
using Natom.Gestion.WebApp.Admin.Backend.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.DTO.Clientes
{
	public class ClienteDTO
	{
		[JsonProperty("encrypted_id")]
		public string EncryptedId { get; set; }

		[JsonProperty("nombre")]
		public string Nombre { get; set; }

		[JsonProperty("apellido")]
		public string Apellido { get; set; }

		[JsonProperty("razonSocial")]
		public string RazonSocial { get; set; }

		[JsonProperty("nombreFantasia")]
		public string NombreFantasia { get; set; }

		[JsonProperty("tipoDocumento_encrypted_id")]
		public string TipoDocumentoEncryptedId { get; set; }

		[JsonProperty("zona_encrypted_id")]
		public string ZonaEncryptedId { get; set; }

		[JsonProperty("zona")]
		public string Zona { get; set; }

		[JsonProperty("tipoDocumento")]
		public string TipoDocumento { get; set; }

		[JsonProperty("numeroDocumento")]
		public string NumeroDocumento { get; set; }

		[JsonProperty("domicilio")]
		public string Domicilio { get; set; }

		[JsonProperty("entreCalles")]
		public string EntreCalles { get; set; }

		[JsonProperty("localidad")]
		public string Localidad { get; set; }

		[JsonProperty("esEmpresa")]
		public bool EsEmpresa { get; set; }

		[JsonProperty("contactoTelefono1")]
		public string ContactoTelefono1 { get; set; }

		[JsonProperty("contactoTelefono2")]
		public string ContactoTelefono2 { get; set; }

		[JsonProperty("contactoEmail1")]
		public string ContactoEmail1 { get; set; }

		[JsonProperty("contactoEmail2")]
		public string ContactoEmail2 { get; set; }

		[JsonProperty("contactoObservaciones")]
		public string ContactoObservaciones { get; set; }

		[JsonProperty("activo")]
		public bool Activo { get; set; }

		public ClienteDTO From(Cliente entity)
		{
			EncryptedId = EncryptionService.Encrypt<Cliente>(entity.ClienteId);
			Nombre = entity.Nombre;
			Apellido = entity.Apellido;
			RazonSocial = entity.RazonSocial;
			NombreFantasia = entity.NombreFantasia;
			TipoDocumentoEncryptedId = EncryptionService.Encrypt<TipoDocumento>(entity.TipoDocumentoId);
			TipoDocumento = entity.TipoDocumento?.Descripcion;
			NumeroDocumento = entity.NumeroDocumento;
			Domicilio = entity.Domicilio;
			EntreCalles = entity.EntreCalles;
			Localidad = entity.Localidad;
			EsEmpresa = entity.EsEmpresa;
			ContactoTelefono1 = entity.ContactoTelefono1;
			ContactoTelefono2 = entity.ContactoTelefono2;
			ContactoEmail1 = entity.ContactoEmail1;
			ContactoEmail2 = entity.ContactoEmail2;
			ContactoObservaciones = entity.ContactoObservaciones;
			Activo = entity.Activo;
			Zona = entity.Zona?.Descripcion;
			ZonaEncryptedId = EncryptionService.Encrypt<Zona>(entity.ZonaId) ?? "";

			return this;
		}

		public Cliente ToModel()
		{
			return new Cliente
			{
				ClienteId = EncryptionService.Decrypt<int, Cliente>(EncryptedId),
				Nombre = Nombre,
				Apellido = Apellido,
				RazonSocial = RazonSocial,
				NombreFantasia = NombreFantasia,
				TipoDocumentoId = EncryptionService.Decrypt<int, TipoDocumento>(TipoDocumentoEncryptedId),
				NumeroDocumento = NumeroDocumento,
				Domicilio = Domicilio,
				EntreCalles = EntreCalles,
				Localidad = Localidad,
				EsEmpresa = EsEmpresa,
				ContactoTelefono1 = ContactoTelefono1,
				ContactoTelefono2 = ContactoTelefono2,
				ContactoEmail1 = ContactoEmail1,
				ContactoEmail2 = ContactoEmail2,
				ContactoObservaciones = ContactoObservaciones,
				Activo = Activo,
				ZonaId = EncryptionService.Decrypt<int, Zona>(ZonaEncryptedId)
		};
		}
	}
}
