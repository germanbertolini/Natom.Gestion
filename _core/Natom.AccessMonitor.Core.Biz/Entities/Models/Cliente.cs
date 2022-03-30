using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Models
{
	[Table("Cliente")]
	public class Cliente
	{
		[Key]
		public int ClienteId { get; set; }
		public bool EsEmpresa { get; set; }
		public string Nombre { get; set; }
		public string Apellido { get; set; }
		public string RazonSocial { get; set; }
		public string NombreFantasia { get; set; }

		public DateTime RegisterAt { get; set; }

		public int TipoDocumentoId { get; set; }
		public TipoDocumento TipoDocumento { get; set; }
		public string NumeroDocumento { get; set; }

		public int? ZonaId { get; set; }
		public Zona Zona { get; set; }

		public string Domicilio { get; set; }
		public string EntreCalles { get; set; }
		public string Localidad { get; set; }

		public string ContactoTelefono1 { get; set; }
		public string ContactoTelefono2 { get; set; }
		public string ContactoEmail1 { get; set; }
		public string ContactoEmail2 { get; set; }
		public string ContactoObservaciones { get; set; }

		public bool Activo { get; set; }

		[NotMapped]
		public int CantidadFiltrados { get; set; }
	}
}
