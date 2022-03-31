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
        public int Confirmado { get; set; }
        public int Reservado { get; set; }
        public int Real { get; set; }
    }
}
