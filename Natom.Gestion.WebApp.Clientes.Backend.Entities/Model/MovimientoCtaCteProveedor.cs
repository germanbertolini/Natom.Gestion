using Natom.Extensions.Auth.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("MovimientoCtaCteProveedor")]
	public class MovimientoCtaCteProveedor
	{
		[Key]
		public int MovimientoCtaCteProveedorId { get; set; }
		public DateTime FechaHora { get; set; }

		public int ProveedorId { get; set; }

		public int? UsuarioId { get; set; }

		[NotMapped]
		public Usuario Usuario { get; set; }

		public int? CompraMovimientoStockId { get; set; }
		[ForeignKey("CompraMovimientoStockId")]
		public MovimientoStock CompraMovimientoStock { get; set; }

		public string Tipo { get; set; }  //(C) REDITO / (D) EBITO
		public decimal Importe { get; set; }

		public string Observaciones { get; set; }

		[NotMapped]
		public int CantidadFiltrados { get; set; }

	}
}
