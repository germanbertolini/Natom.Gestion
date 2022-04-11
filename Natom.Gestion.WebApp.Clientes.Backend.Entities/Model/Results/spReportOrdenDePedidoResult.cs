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
	public class spReportOrdenDePedidoResult
    {
		public string Numero { get; set; }
		public DateTime FechaHora { get; set; }
		public string Cliente { get; set; }
		public string ClienteDocumento { get; set; }
		public string ClienteDomicilio { get; set; }
		public string ClienteLocalidad { get; set; }
		public string RemitoNumero { get; set; }
		public string VentaNumero { get; set; }
		public string VentaComprobante { get; set; }
		public string EntregaFecha { get; set; }
		public string EntregaRangoHorario { get; set; }
		public string EntregaDomicilio { get; set; }
		public string EntregaEntreCalles { get; set; }
		public string EntregaLocalidad { get; set; }
		public string EntregaTelefono1 { get; set; }
		public string EntregaTelefono2 { get; set; }
		public string EntregaObservaciones { get; set; }
		public int CargadoPorUsuarioId { get; set; }
		[NotMapped] public string CargadoPor { get; set; }
		public string Observaciones { get; set; }
		public string Anulado { get; set; }
		public string Facturado { get; set; }
		public string Pagado { get; set; }
		public string Entregado { get; set; }
		public decimal MontoTotal { get; set; }
		public decimal PesoTotalEnKilogramos { get; set; }
		public string DetalleCodigo { get; set; }
		public string DetalleDescripcion { get; set; }
		public decimal DetalleCantidad { get; set; }
		public decimal DetallePesoUnitarioEnKilogramos { get; set; }
		public decimal? DetallePrecioUnitario { get; set; }
		public decimal DetallePesoTotalEnKilogramos { get; set; }
		public decimal? DetallePrecioTotal { get; set; }
		public string DetalleDeposito { get; set; }
		public string DetalleListaDePrecios { get; set; }
	}
}
