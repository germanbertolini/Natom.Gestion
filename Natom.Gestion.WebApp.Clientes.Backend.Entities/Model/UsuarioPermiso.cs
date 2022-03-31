using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
    [Table("UsuarioPermiso")]
    public class UsuarioPermiso
    {
        [Key]
        public int UsuarioPermisoId { get; set; }

	    public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string PermisoId { get; set; }
        public Permiso Permiso { get; set; }
    }
}
