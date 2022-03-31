using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
	[Keyless]
	public class spPreciosListResult
    {
		public int? ProductoPrecioId { get; set; }
		public int ProductoId { get; set; }
		public string ProductoDescripcion { get; set; }
		public int ListaDePreciosId { get; set; }
		public string ListaDePrecioDescripcion { get; set; }
		public DateTime AplicaDesdeFechaHora { get; set; }
		public decimal Precio { get; set; }
		public bool ListaDePreciosEsPorcentual { get; set; }
		public int CantidadRegistros { get; set; }

		[NotMapped]
		public int CantidadFiltrados { get; set; }
	}
}
