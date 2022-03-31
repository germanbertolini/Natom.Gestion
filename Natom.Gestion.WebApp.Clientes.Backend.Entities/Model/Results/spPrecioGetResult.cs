using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
	[Keyless]
	public class spPrecioGetResult
    {
		public int? ProductoPrecioId { get; set; }
		public decimal Precio { get; set; }
		public int ListaDePreciosId { get; set; }
	}
}
