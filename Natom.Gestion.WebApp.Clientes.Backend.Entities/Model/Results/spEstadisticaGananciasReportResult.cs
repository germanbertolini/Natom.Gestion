using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
    [Keyless]
    public class spEstadisticaGananciasReportResult
    {
        public DateTime Fecha { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalCostosVentas { get; set; }
        public decimal TotalEgresosCajaDiaria { get; set; }
        public decimal TotalGanancias { get; set; }
    }
}
