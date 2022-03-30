using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Natom.AccessMonitor.Core.Biz.Entities.Models
{
    [Table("TipoDocumento")]
    public class TipoDocumento
    {
        [Key]
        public int TipoDocumentoId { get; set; }
        public string Descripcion { get; set; }
    }
}