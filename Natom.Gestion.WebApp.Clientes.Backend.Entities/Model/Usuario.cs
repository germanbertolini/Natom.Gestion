using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("Usuario")]
	public class Usuario
    {
		[Key]
		public int UsuarioId { get; set; }
		public string Nombre { get; set; }
		public string Apellido { get; set; }
		public string Email { get; set; }
		public string Clave { get; set; }
		public DateTime? FechaHoraConfirmacionEmail { get; set; }
		public string SecretConfirmacion { get; set; }
		public DateTime FechaHoraAlta { get; set; }
		public DateTime? FechaHoraBaja { get; set; }

		public List<UsuarioPermiso> Permisos { get; set; }

		[NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
