using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("HistoricoCambios")]
	public class HistoricoCambios
    {
		[Key]
		public int HistoricoCambiosId { get; set; }
		
		public int? UsuarioId { get; set; }
		public Usuario Usuario { get; set; }

		public DateTime FechaHora { get; set; }
		public string EntityName { get; set; }
		public int? EntityId { get; set; }
		public string Accion { get; set; }

		public List<HistoricoCambiosMotivo> Motivos { get; set; }
	}
}
