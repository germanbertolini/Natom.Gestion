using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Models
{
    [Table("Goal")]
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }

        public int PlaceId { get; set; }
        public Place Place { get; set; }

        public DateTime? RemovedAt { get; set; }

        [NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
