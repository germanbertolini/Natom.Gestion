using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model
{
	[Table("Producto")]
	public class Producto
    {
		[Key]
		public int ProductoId { get; set; }

		public int MarcaId { get; set; }
		public Marca Marca { get; set; }

		public string Codigo { get; set; }

		public string DescripcionCorta { get; set; }
		public string DescripcionLarga { get; set; }
		
		public int UnidadPesoId { get; set; }
		public UnidadPeso UnidadPeso { get; set; }
		public decimal PesoUnitario { get; set; }

		public decimal? CostoUnitario { get; set; }

		public int? ProveedorId { get; set; }
		public Proveedor Proveedor { get; set; }

		public bool MueveStock { get; set; }

		public bool Activo { get; set; }

		public int CategoriaProductoId { get; set; }
		public CategoriaProducto CategoriaProducto { get; set; }

		[NotMapped]
        public int CantidadFiltrados { get; set; }
    }
}
