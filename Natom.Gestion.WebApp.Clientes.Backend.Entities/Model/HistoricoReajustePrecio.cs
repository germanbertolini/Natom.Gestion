using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("HistoricoReajustePrecio")]
	public class HistoricoReajustePrecio
    {
		[Key]
		public int HistoricoReajustePrecioId { get; set; }
		public DateTime FechaHora { get; set; }

		public int? UsuarioId { get; set; }
		public Usuario Usuario { get; set; }

		public bool EsIncremento { get; set; }
		public bool EsPorcentual { get; set; }
		public decimal Valor { get; set; }

		public int AplicoMarcaId { get; set; }
		[ForeignKey("AplicoMarcaId")]
		public virtual Marca AplicoMarca { get; set; }

		public DateTime? FechaHoraBaja { get; set; }

		public int? AplicoListaDePreciosId { get; set; }
		[ForeignKey("AplicoListaDePreciosId")]
		public virtual ListaDePrecios AplicoListaDePrecios { get; set; }
		public DateTime AplicaDesdeFechaHora { get; set; }
    }
}
