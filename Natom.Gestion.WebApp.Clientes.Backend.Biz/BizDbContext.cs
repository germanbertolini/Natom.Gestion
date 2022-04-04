using Microsoft.EntityFrameworkCore;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model.Results;
using System;

namespace Natom.Gestion.WebApp.Clientes.Backend.Biz
{
    public class BizDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<HistoricoCambios> HistoricosCambios { get; set; }
        public DbSet<HistoricoCambiosMotivo> HistoricosCambiosMotivos { get; set; }
        public DbSet<HistoricoReajustePrecio> HistoricosReajustePrecios { get; set; }
        public DbSet<ListaDePrecios> ListasDePrecios { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Zona> Zonas { get; set; }
        public DbSet<Transporte> Transportes { get; set; }
        public DbSet<MovimientoCajaDiaria> MovimientosCajaDiaria { get; set; }
        public DbSet<MovimientoCajaFuerte> MovimientosCajaFuerte { get; set; }
        public DbSet<MovimientoCtaCteCliente> MovimientosCtaCteCliente { get; set; }
        public DbSet<MovimientoCtaCteProveedor> MovimientosCtaCteProveedor { get; set; }
        public DbSet<MovimientoStock> MovimientosStock { get; set; }
        public DbSet<OrdenDePedido> OrdenesDePedido { get; set; }
        public DbSet<OrdenDePedidoDetalle> OrdenesDePedidoDetalle { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoPrecio> ProductosPrecios { get; set; }
        public DbSet<RangoHorario> RangosHorario { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }
        public DbSet<TipoResponsable> TiposResponsable { get; set; }
        public DbSet<UnidadPeso> UnidadesPeso { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentasDetalle { get; set; }


        public DbSet<spPrecioGetResult> spPrecioGetResult { get; set; }
        public DbSet<spPreciosListResult> spPreciosListResult { get; set; }
        public DbSet<spMovimientosStockListResult> spMovimientosStockListResult { get; set; }
        public DbSet<spReportOrdenDePedidoResult> spReportOrdenDePedidoResult { get; set; }
        public DbSet<spReportRemitoResult> spReportRemitoResult { get; set; }
        public DbSet<spReportVentaResult> spReportVentaResult { get; set; }
        public DbSet<spVentasPorProductoProveedorReportResult> spVentasPorProductoProveedorReportResult { get; set; }
        public DbSet<spClientesQueNoCompranDesdeFechaReportResult> spClientesQueNoCompranDesdeFechaReportResult { get; set; }
        public DbSet<spKilosCompradosPorProveedorReportResult> spKilosCompradosPorProveedorReportResult { get; set; }
        public DbSet<spVentasRepartoVsMostradorReportResult> spVentasRepartoVsMostradorReportResult { get; set; }
        public DbSet<spTotalVendidoPorListaDePreciosReportResult> spTotalVendidoPorListaDePreciosReportResult { get; set; }
        public DbSet<spEstadisticaComprasReportResult> spEstadisticaComprasReportResult { get; set; }
        public DbSet<spEstadisticaGananciasReportResult> spEstadisticaGananciasReportResult { get; set; }
        public DbSet<spPreciosListaReportResult> spPreciosListaReportResult { get; set; }
        public DbSet<spStockListaReportResult> spStockListaReportResult { get; set; }

        public BizDbContext(DbContextOptions<BizDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder
            //    .Entity<spBookingGetForCalendarResult>(builder => builder.HasNoKey())
            //    .Entity<spPropertiesGetForAgreementsResult>(builder => builder.HasNoKey());
        }
    }
}
