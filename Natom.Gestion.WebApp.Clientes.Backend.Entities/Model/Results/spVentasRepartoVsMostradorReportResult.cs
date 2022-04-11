using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
    [Keyless]
    public class spVentasRepartoVsMostradorReportResult
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal CantidadMostrador { get; set; }
        public decimal VariacionCantidadMostrador { get; set; }
        public string ColorVariacionCantidadMostrador { get; set; }
        public decimal KilosMostrador { get; set; }
        public decimal VariacionKilosMostrador { get; set; }
        public string ColorVariacionKilosMostrador { get; set; }
        public decimal CantidadReparto { get; set; }
        public decimal VariacionCantidadReparto { get; set; }
        public string ColorVariacionCantidadReparto { get; set; }
        public decimal KilosReparto { get; set; }
        public decimal VariacionKilosReparto { get; set; }
        public string ColorVariacionKilosReparto { get; set; }
    }
}
