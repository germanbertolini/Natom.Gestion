using Natom.Extensions.Auth.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("Venta")]
	public class Venta
	{
		[Key]
		public int VentaId { get; set; }
		
		public int NumeroVenta { get; set; }

		public int ClienteId { get; set; }
		public Cliente Cliente { get; set; }

		public DateTime FechaHoraVenta { get; set; }

		public int? UsuarioId { get; set; }
		public Usuario Usuario { get; set; }

		public string MedioDePago { get; set; }
		public string PagoReferencia { get; set; }

		public string TipoFactura { get; set; }
		public string NumeroFactura { get; set; }
		public bool Activo { get; set; }
		public string Observaciones { get; set; }

		public decimal MontoTotal { get; set; }

		public int PesoTotalEnGramos { get; set; }

		public List<VentaDetalle> Detalle { get; set; }

		public List<MovimientoCajaDiaria> ComposicionPagoCajaDiaria { get; set; }
		public List<MovimientoCajaFuerte> ComposicionPagoCajaFuerte { get; set; }
		public List<MovimientoCtaCteCliente> ComposicionPagoCuentaCorriente { get; set; }

		[NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}