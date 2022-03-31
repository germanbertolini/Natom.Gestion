using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("ProductoPrecio")]
	public class ProductoPrecio
    {
		[Key]
		public int ProductoPrecioId { get; set; }
		
		public int ProductoId { get; set; }
		public Producto Producto { get; set; }

		public int? ListaDePreciosId { get; set; }
		public ListaDePrecios ListaDePrecios { get; set; }

		public decimal Precio { get; set; }
		public DateTime AplicaDesdeFechaHora { get; set; }

		public DateTime? FechaHoraBaja { get; set; }

		public int? HistoricoReajustePrecioId { get; set; } //POR SI FUE PRODUCTO DE UN REAJUSTE POR MARCA CON PORCENTAJE
		public HistoricoReajustePrecio HistoricoReajustePrecio { get; set; }
	}
}
