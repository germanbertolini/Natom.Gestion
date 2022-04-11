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
	[Table("OrdenDePedido")]
	public class OrdenDePedido
    {
		[Key]
		public int OrdenDePedidoId { get; set; }
		public int NumeroPedido { get; set; }

		public int ClienteId { get; set; }
		public Cliente Cliente { get; set; }

		public DateTime FechaHoraPedido { get; set; }

		public DateTime? EntregaEstimadaFecha { get; set; }
		public int? EntregaEstimadaRangoHorarioId { get; set; }
		[ForeignKey("EntregaEstimadaRangoHorarioId")]
		public RangoHorario EntregaEstimadaRangoHorario { get; set; }
		public bool RetiraPersonalmente { get; set; }
		public string EntregaDomicilio { get; set; }
		public string EntregaEntreCalles { get; set; }
		public string EntregaLocalidad { get; set; }
		public string EntregaTelefono1 { get; set; }
		public string EntregaTelefono2 { get; set; }
		public string EntregaObservaciones { get; set; }

		public int? UsuarioId { get; set; }

		[NotMapped]
		public Usuario Usuario { get; set; }

		public string NumeroRemito { get; set; }

		public int? VentaId { get; set; }
		public Venta Venta { get; set; }

		public bool? Activo { get; set; }
		public string Observaciones { get; set; }

		public DateTime? PreparacionFechaHoraInicio { get; set; }
		public DateTime? PreparacionFechaHoraFin { get; set; }
		public int? PreparacionUsuarioId { get; set; }

		[NotMapped]
		public Usuario PreparacionUsuario { get; set; }

		public DateTime? DespachoFechaHora { get; set; }
		public int? DespachoUsuarioId { get; set; }
		[ForeignKey("DespachoUsuarioId")]
		public Usuario DespachoUsuario { get; set; }

		public int? DespachoTransporteId { get; set; }
		[ForeignKey("DespachoTransporteId")]
		public Transporte DespachoTransporte { get; set; }

		public DateTime? MarcoEntregaFechaHora { get; set; }
		public int? MarcoEntregaUsuarioId { get; set; }
		[ForeignKey("MarcoEntregaUsuarioId")]
		public Usuario MarcoEntregaUsuario { get; set; }

		public decimal PesoTotalEnGramos { get; set; }
		public decimal? MontoTotal { get; set; }

		public string MedioDePago { get; set; }
		public string PagoReferencia { get; set; }

		public List<OrdenDePedidoDetalle> Detalle { get; set; }

		[NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
