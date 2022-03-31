using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models
{
    [Table("Zona")]
    public class Zona
    {
        [Key]
        public int ZonaId { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }

        [NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}