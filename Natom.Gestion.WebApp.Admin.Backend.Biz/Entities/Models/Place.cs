using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models
{
    [Table("Place")]
    public class Place
    {
        [Key]
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Cliente Cliente { get; set; }

        public DateTime? RemovedAt { get; set; }

        public List<Goal> Goals { get; set; }
        public List<ConfigTolerancia> ConfigTolerancias { get; set; }


        [NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}