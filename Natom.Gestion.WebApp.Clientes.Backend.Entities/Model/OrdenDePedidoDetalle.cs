using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
    [Table("OrdenDePedidoDetalle")]
    public class OrdenDePedidoDetalle
    {
        [Key]
        public int OrdenDePedidoDetalleId { get; set; }

	    public int OrdenDePedidoId { get; set; }
        public OrdenDePedido OrdenDePedido { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public decimal Cantidad { get; set; }
        public decimal? CantidadEntregada { get; set; }

        public int DepositoId { get; set; }
        public Deposito Deposito { get; set; }

        public int? MovimientoStockId { get; set; }
        public MovimientoStock MovimientoStock { get; set; }

        public int? ListaDePreciosId { get; set; }
        [ForeignKey("ListaDePreciosId")]
        public ListaDePrecios ListaDePrecios { get; set; }

        public decimal? Precio { get; set; }

        public int PesoUnitarioEnGramos { get; set; }
    }
}