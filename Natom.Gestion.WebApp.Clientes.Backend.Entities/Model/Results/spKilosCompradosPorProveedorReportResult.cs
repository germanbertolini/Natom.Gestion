using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
    [Keyless]
    public class spKilosCompradosPorProveedorReportResult
    {
        public decimal TotalKilosComprados { get; set; }
        public string Proveedor { get; set; }
        public string Documento { get; set; }
    }
}
