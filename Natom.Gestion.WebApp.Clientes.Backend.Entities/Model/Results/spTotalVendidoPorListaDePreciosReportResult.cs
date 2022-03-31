using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
    [Keyless]
    public class spTotalVendidoPorListaDePreciosReportResult
    {
        public string ListaDePrecios { get; set; }
        public decimal MontoVendido { get; set; }
        public decimal KilosVendidos { get; set; }
    }
}
