using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Ventas;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers
{
    public class ReportingManager : BaseManager
    {
        public ReportingManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public List<spVentasPorProductoProveedorReportResult> ObtenerDataVentasPorProductoProveedorReport(int? productoId, int? proveedorId, DateTime desde, DateTime hasta)
        {
            return _db.spVentasPorProductoProveedorReportResult.FromSqlRaw("spVentasPorProductoProveedorReport {0}, {1}, {2}, {3}", productoId, proveedorId, desde, hasta).AsEnumerable().ToList();
        }

        public List<spClientesQueNoCompranDesdeFechaReportResult> ObtenerDataClientesQueNoCompranDesdeFechaReport(DateTime desde)
        {
            return _db.spClientesQueNoCompranDesdeFechaReportResult.FromSqlRaw("spClientesQueNoCompranDesdeFechaReport {0}", desde).AsEnumerable().ToList();
        }
        
        public List<spKilosCompradosPorProveedorReportResult> ObtenerDataKilosCompradosPorProveedorReport(DateTime? desde)
        {
            return _db.spKilosCompradosPorProveedorReportResult.FromSqlRaw("spKilosCompradosPorProveedorReport {0}", desde).AsEnumerable().ToList();
        }
        
        public List<spVentasRepartoVsMostradorReportResult> ObtenerDataVentasRepartoVsMostradorReport(DateTime? desde)
        {
            return _db.spVentasRepartoVsMostradorReportResult.FromSqlRaw("spVentasRepartoVsMostradorReport {0}", desde).AsEnumerable().ToList();
        }

        public List<spTotalVendidoPorListaDePreciosReportResult> ObtenerDataTotalVendidoPorListaDePreciosReport(DateTime? desde)
        {
            return _db.spTotalVendidoPorListaDePreciosReportResult.FromSqlRaw("spTotalVendidoPorListaDePreciosReport {0}", desde).AsEnumerable().ToList();
        }

        public List<spEstadisticaComprasReportResult> ObtenerDataEstadisticaComprasReport(DateTime desde, DateTime? hasta)
        {
            return _db.spEstadisticaComprasReportResult.FromSqlRaw("spEstadisticaComprasReport {0}, {1}", desde, hasta).AsEnumerable().ToList();
        }

        public List<spEstadisticaGananciasReportResult> ObtenerDataEstadisticaGananciasReport(DateTime desde, DateTime? hasta)
        {
            return _db.spEstadisticaGananciasReportResult.FromSqlRaw("spEstadisticaGananciasReport {0}, {1}", desde, hasta).AsEnumerable().ToList();
        }

        public List<spPreciosListaReportResult> ObtenerDataListaDePreciosReport(int listaDePreciosId)
        {
            return _db.spPreciosListaReportResult.FromSqlRaw("spPreciosListaReport {0}", listaDePreciosId).AsEnumerable().ToList();
        }

        public List<spStockListaReportResult> ObtenerDataListaStockReport(int? depositoId)
        {
            return _db.spStockListaReportResult.FromSqlRaw("spStockListaReport {0}", depositoId).AsEnumerable().ToList();
        }
    }
}
