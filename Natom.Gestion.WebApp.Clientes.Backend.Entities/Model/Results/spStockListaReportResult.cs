using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
    [Keyless]
    public class spStockListaReportResult
    {
        public string Deposito { get; set; }
        public string Categoria { get; set; }
        public string Producto { get; set; }
        public decimal Confirmado { get; set; }
        public decimal Reservado { get; set; }
        public decimal Real { get; set; }
    }
}
