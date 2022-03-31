using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
    [Table("Marca")]
    public class Marca
    {
        [Key]
        public int MarcaId { get; set; }
	    public string Descripcion { get; set; }
        public bool Activo { get; set; }

        [NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
