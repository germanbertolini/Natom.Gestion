using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Models
{
	[Table("Docket")]
    public class Docket
    {
		[Key]
		public int DocketId { get; set; }

		public int ClientId { get; set; }
		[ForeignKey("ClientId")]
		public Cliente Cliente { get; set; }

		public int PlaceId { get; set; }
		public Place Place { get; set; }

		public string DocketNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string DNI { get; set; }

		public int TitleId { get; set; }
		public Title Title { get; set; }

		public decimal? HourValue { get; set; }
		public decimal? ExtraHourValue { get; set; }

		public bool Active { get; set; }

		public List<DocketRange> Ranges { get; set; }


		[NotMapped]
		public int CantidadFiltrados { get; set; }
	}
}
