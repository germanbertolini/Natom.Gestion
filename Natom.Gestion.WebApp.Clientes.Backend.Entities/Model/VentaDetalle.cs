using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("VentaDetalle")]
	public class VentaDetalle
	{
		[Key]
		public int VentaDetalleId { get; set; }

		public int VentaId { get; set; }
		public Venta Venta { get; set; }

		public int ProductoId { get; set; }
		public Producto Producto { get; set; }

		public decimal Cantidad { get; set; }

		public int DepositoId { get; set; }
		public Deposito Deposito { get; set; }

		public int? ListaDePreciosId { get; set; }
		[ForeignKey("ListaDePreciosId")]
		public ListaDePrecios ListaDePrecios { get; set; }

		public decimal Precio { get; set; }

		public int? PesoUnitarioEnGramos { get; set; }

		public int? OrdenDePedidoId { get; set; }
		public OrdenDePedido OrdenDePedido { get; set; }

		public int? OrdenDePedidoDetalleId { get; set; }
		public OrdenDePedidoDetalle OrdenDePedidoDetalle { get; set; }

		public string NumeroRemito { get; set; }
	}
}