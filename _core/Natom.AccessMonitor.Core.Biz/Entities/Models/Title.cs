using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.AccessMonitor.Core.Biz.Entities.Models
{
    [Table("Title")]
    public class Title
    {
        [Key]
        public int TitleId { get; set; }
        public string Name { get; set; }
        public int ClienteId { get; set; }
        public DateTime? RemovedAt { get; set; }

        [NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
