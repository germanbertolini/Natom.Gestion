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
	[Table("MovimientoStock")]
	public class MovimientoStock
    {
		[Key]
		public int MovimientoStockId { get; set; }

		public int ProductoId { get; set; }
		public Producto Producto { get; set; }

		public DateTime FechaHora { get; set; }

		public int? UsuarioId { get; set; }
		public Usuario Usuario { get; set; }

		public string Tipo { get; set; } //(I) NGRESO / (E) GRESO
		public int Cantidad { get; set; }

		public int DepositoId { get; set; }
		public Deposito Deposito { get; set; }

		public string Observaciones { get; set; }

		public DateTime? ConfirmacionFechaHora { get; set; }
		public int? ConfirmacionUsuarioId { get; set; }
		[ForeignKey("ConfirmacionUsuarioId")]
		public Usuario ConfirmacionUsuario { get; set; }

		public bool EsCompra { get; set; }
		public int? ProveedorId { get; set; }
		public Proveedor Proveedor { get; set; }
		public decimal? CostoUnitario { get; set; }

		public DateTime? FechaHoraControlado { get; set; }
		public int? ControladoUsuarioId { get; set; }
		[ForeignKey("ControladoUsuarioId")]
		public Usuario ControladoUsuario { get; set; }
	}
}
