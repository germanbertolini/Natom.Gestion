using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models
{
	[Table("ConfigTolerancia")]
    public class ConfigTolerancia
    {
		[Key]
		public int ConfigToleranciaId { get; set; }
		
		public int ClienteId { get; set; }
		public Cliente Cliente { get; set; }

		public int PlaceId { get; set; }
		public Place Place { get; set; }

		public int IngresoToleranciaMins { get; set; }
		public int EgresoToleranciaMins { get; set; }
		public TimeSpan AlmuerzoHorarioDesde { get; set; }
		public TimeSpan AlmuerzoHorarioHasta { get; set; }
		public int AlmuerzoTiempoLimiteMins { get; set; }

		public int ConfiguroUsuarioId { get; set; }
		public DateTime ConfiguroFechaHora { get; set; }

		public DateTime AplicaDesde { get; set; }
		public DateTime? AplicaHasta { get; set; }

		[NotMapped]
		public int CantidadFiltrados { get; set; }

        public string GetStatus()
        {
			return AplicaDesde.Date <= DateTime.Now.Date && (!AplicaHasta.HasValue || (AplicaHasta.HasValue && AplicaHasta >= DateTime.Now.Date)) ? "VIGENTE" :
						AplicaDesde.Date > DateTime.Now.Date ? "PROGRAMADO"
						: "CADUCADO";

		}
    }
}
