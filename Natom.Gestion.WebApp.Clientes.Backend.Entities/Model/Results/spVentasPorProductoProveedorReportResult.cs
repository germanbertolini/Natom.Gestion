using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results
{
    [Keyless]
    public class spVentasPorProductoProveedorReportResult
    {
        public DateTime FechaHora { get; set; }
        public string VendidoPor { get; set; }
        public string Operacion { get; set; }
        public decimal PesoTotalKilos { get; set; }
        public string Remito { get; set; }
        public string Venta { get; set; }
        public string ComprobanteVenta { get; set; }
        public string Proveedor { get; set; }
        public string Producto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
