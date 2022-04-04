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
	[Table("MovimientoCajaFuerte")]
	public class MovimientoCajaFuerte
    {
		[Key]
		public int MovimientoCajaFuerteId { get; set; }
		public DateTime FechaHora { get; set; }
		
		public int? UsuarioId { get; set; }
		public Usuario Usuario { get; set; }

		public string Tipo { get; set; }  //(C) REDITO / (D) EBITO
		public decimal Importe { get; set; }

		public string Observaciones { get; set; }

		public bool EsCheque { get; set; }

		public int? VentaId { get; set; }
		public Venta Venta { get; set; }

		public string MedioDePago { get; set; }
		public string Referencia { get; set; }

		[NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
