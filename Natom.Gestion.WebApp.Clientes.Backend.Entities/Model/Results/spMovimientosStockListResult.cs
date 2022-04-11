using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
    [Keyless]
    public class spMovimientosStockListResult
    {
        public int MovimientoStockId { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime? FechaHoraControlado { get; set; }
        public string Deposito { get; set; }
        public string Producto { get; set; }
        public string Tipo { get; set; }
        public decimal? Movido { get; set; }
        public decimal? Reservado { get; set; }
        public string Observaciones { get; set; }
        public decimal Stock { get; set; }
        public int CantidadRegistros { get; set; }
        public int CantidadFiltrados { get; set; }
    }
}
