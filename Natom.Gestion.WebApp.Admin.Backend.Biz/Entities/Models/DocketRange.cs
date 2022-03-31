using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models
{
    [Table("DocketRange")]
    public class DocketRange
    {
        [Key]
        public int DocketRangeId { get; set; }

        public int DocketId { get; set; }
        public Docket Docket { get; set; }

        public int DayOfWeek { get; set; }
        public TimeSpan? From { get; set; }
        public TimeSpan? To { get; set; }
    }
}