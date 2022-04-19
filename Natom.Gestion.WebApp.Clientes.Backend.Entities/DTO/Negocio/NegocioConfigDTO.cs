using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Negocio
{
    public class NegocioConfigDTO
    {
		[JsonProperty("razon_social")]
		public string RazonSocial { get; set; }

		[JsonProperty("fecha_membresia")]
		public DateTime FechaMembresia { get; set; }

		[JsonProperty("nombre_fantasia")]
		public string NombreFantasia { get; set; }

		[JsonProperty("tipo_documento")]
		public string TipoDocumento { get; set; }

		[JsonProperty("numero_documento")]
		public string NumeroDocumento { get; set; }

		[JsonProperty("domicilio")]
		public string Domicilio { get; set; }

		[JsonProperty("localidad")]
		public string Localidad { get; set; }

		[JsonProperty("telefono")]
		public string Telefono { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("logo_base64")]
		public string LogoBase64 { get; set; }

		public NegocioConfigDTO From(NegocioConfig data)
		{
			this.FechaMembresia = data.FechaMembresia;
			this.RazonSocial = data.RazonSocial;
			this.NombreFantasia = data.NombreFantasia;
			this.TipoDocumento = data.TipoDocumento;
			this.NumeroDocumento = data.NumeroDocumento;
			this.Domicilio = data.Domicilio;
			this.Localidad = data.Localidad;
			this.Telefono = data.Telefono;
			this.Email = data.Email;
			this.LogoBase64 = data.LogoBase64;

			return this;
        }

		public NegocioConfig ToModel()
		{
			var model = new NegocioConfig();
			model.FechaMembresia = this.FechaMembresia;
			model.RazonSocial = this.RazonSocial;
			model.NombreFantasia = this.NombreFantasia;
			model.TipoDocumento = this.TipoDocumento;
			model.NumeroDocumento = this.NumeroDocumento;
			model.Domicilio = this.Domicilio;
			model.Localidad = this.Localidad;
			model.Telefono = this.Telefono;
			model.Email = this.Email;
			model.LogoBase64 = this.LogoBase64;

			return model;
		}
	}
}
