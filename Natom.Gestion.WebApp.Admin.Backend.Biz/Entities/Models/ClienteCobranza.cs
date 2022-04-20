using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models
{
    [Table("ClienteCobranza")]
    public class ClienteCobranza
    {
        [Key]
        public int ClienteCobranzaId { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public decimal Monto { get; set; }

        public DateTime FechaHora { get; set; }

        public int ClienteMontoId { get; set; }
        public ClienteMonto ClienteMonto { get; set; }
    }
}