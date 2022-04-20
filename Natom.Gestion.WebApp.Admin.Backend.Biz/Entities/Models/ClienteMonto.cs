using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models
{
	[Table("ClienteMonto")]
	public class ClienteMonto
	{
		[Key]
		public int ClienteMontoId { get; set; }

		public int ClienteId { get; set; }
		public Cliente Cliente { get; set; }

		public decimal Monto { get; set; }

		public DateTime Desde { get; set; }

		public int UsuarioId { get; set; }

		public DateTime? FechaHoraAnulado { get; set; }
    }
}
