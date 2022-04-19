using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
    public class NegocioConfig
	{
		public int NegocioConfigId { get; set; }
		public DateTime FechaMembresia { get; set; }
		public string RazonSocial { get; set; }
		public string NombreFantasia { get; set; }
		public string TipoDocumento { get; set; }
		public string NumeroDocumento { get; set; }
		public string Domicilio { get; set; }
		public string Localidad { get; set; }
		public string Telefono { get; set; }
		public string Email { get; set; }
		public string LogoBase64 { get; set; }
    }
}
