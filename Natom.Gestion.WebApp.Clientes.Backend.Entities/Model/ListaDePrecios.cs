using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
    [Table("ListaDePrecios")]
    public class ListaDePrecios
    {
        [Key]
        public int ListaDePreciosId { get; set; }
	    public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public bool EsPorcentual { get; set; }
        public decimal? IncrementoPorcentaje { get; set; }

        public int? IncrementoDeListaDePreciosId { get; set; }
        [ForeignKey("IncrementoDeListaDePreciosId")]
        public ListaDePrecios IncrementoDeListaDePrecios { get; set; }

        [NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
