import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "./guards/auth.guards";
import { HomeComponent } from "./views/home/home.component";
import { MeProfileComponent } from "./views/me/profile/me-profile.component";
import { ErrorPageComponent } from "./views/error-page/error-page.component";
import { UserCrudComponent } from "./views/users/crud/user-crud.component";
import { UsersComponent } from "./views/users/users.component";
import { LoginComponent } from "./views/login/login.component";
import { MarcaCrudComponent } from "./views/marcas/crud/marca-crud.component";
import { MarcasComponent } from "./views/marcas/marcas.component";
import { CajaDiariaComponent } from "./views/cajas/diaria/caja-diaria.component";
import { CajaDiariaNewComponent } from "./views/cajas/diaria/new/caja-diaria-new.component";
import { CajaFuerteComponent } from "./views/cajas/fuerte/caja-fuerte.component";
import { CajaFuerteNewComponent } from "./views/cajas/fuerte/new/caja-fuerte-new.component";
import { CajaTransferenciaComponent } from "./views/cajas/transferencia/caja-transferencia.component";
import { ClienteCrudComponent } from "./views/clientes/crud/cliente-crud.component";
import { ClientesComponent } from "./views/clientes/clientes.component";
import { ProductosComponent } from "./views/productos/productos.component";
import { ProductoCrudComponent } from "./views/productos/crud/producto-crud.component";
import { PreciosComponent } from "./views/precios/precios.component";
import { PrecioCrudComponent } from "./views/precios/crud/precio-crud.component";
import { PreciosReajustesComponent } from "./views/precios/reajustes/precios-reajustes.component";
import { PrecioReajusteCrudComponent } from "./views/precios/reajustes/crud/precio-reajuste-crud.component";
import { StockComponent } from "./views/stock/stock.component";
import { StockNewComponent } from "./views/stock/new/stock-new.component";
import { PedidosComponent } from "./views/pedidos/pedidos.component";
import { PedidoCrudComponent } from "./views/pedidos/crud/pedido-crud.component";
import { UserConfirmComponent } from "./views/users/confirm/user-confirm.component";
import { VentaCrudComponent } from "./views/ventas/crud/venta-crud.component";
import { VentasComponent } from "./views/ventas/ventas.component";
import { ABMMarcasGuard } from "./guards/marcas/abm.marcas.guards";
import { ABMUsuariosGuard } from "./guards/usuarios/abm.usuarios.guards";
import { CajaDiariaVerGuard } from "./guards/cajas/diaria/caja-diaria.ver.guards";
import { CajaDiariaNuevoMovimientoGuard } from "./guards/cajas/diaria/caja-diaria.nuevo.guards";
import { CajaFuerteVerGuard } from "./guards/cajas/fuerte/caja-fuerte.ver.guards";
import { CajaFuerteNuevoMovimientoGuard } from "./guards/cajas/fuerte/caja-fuerte.nuevo.guards";
import { CajaTransferenciaGuard } from "./guards/cajas/caja.transferencia.guards";
import { CRUDClientesGuard } from "./guards/clientes/clientes.crud.guards";
import { VerClientesGuard } from "./guards/clientes/clientes.ver.guards";
import { VerPedidosGuard } from "./guards/pedidos/ver.pedidos.guards";
import { NuevoPedidoGuard } from "./guards/pedidos/nuevo.pedidos.guards";
import { CRUDPreciosGuard } from "./guards/precios/crud.precios.guards";
import { VerPreciosGuard } from "./guards/precios/ver.precios.guards";
import { ReajustarPreciosGuard } from "./guards/precios/reajustar.precios.guards";
import { VerStockGuard } from "./guards/stock/ver.stock.guards";
import { NuevoMovimientoStockGuard } from "./guards/stock/nuevo.stock.guards";
import { VerProductosGuard } from "./guards/productos/ver.productos.guards";
import { CRUDProductosGuard } from "./guards/productos/crud.productos.guards";
import { VerVentasGuard } from "./guards/ventas/ver.ventas.guards";
import { NuevoVentasGuard } from "./guards/ventas/nuevo.ventas.guards";
import { ProveedorCrudComponent } from "./views/proveedores/crud/proveedor-crud.component";
import { VerProveedoresGuard } from "./guards/proveedores/proveedores.ver.guards";
import { CRUDProveedoresGuard } from "./guards/proveedores/proveedores.crud.guards";
import { ProveedoresComponent } from "./views/proveedores/proveedores.component";
import { ReportesListadosVentasPorProductoProveedorComponent } from "./views/reportes/listados/ventas-por-producto-proveedor/reportes-listados-ventas-por-producto-proveedor.component";
import { ClientesQueNoCompranDesdeFechaComponent } from "./views/reportes/listados/clientes-que-no-compran-desde-fecha/clientes-que-no-compran-desde-fecha.component";
import { KilosCompradosPorCadaProveedorComponent } from "./views/reportes/estadistica/kilos-comprados-por-cada-proveedor/kilos-comprados-por-cada-proveedor.component";
import { VentasRepartoVsMostradorComponent } from "./views/reportes/estadistica/ventas-reparto-vs-mostrador/ventas-reparto-vs-mostrador.component";
import { TotalVentasPorListaDePreciosComponent } from "./views/reportes/estadistica/total-ventas-por-lista-de-precios/total-ventas-por-lista-de-precios.component";
import { ReportesEstadisticaComprasComponent } from "./views/reportes/estadistica/compras/reportes-estadistica-compras.component";
import { ReportesEstadisticaGananciasComponent } from "./views/reportes/estadistica/ganancias/reportes-estadistica-ganancias.component";
import { KilosCompradosPorProveedorGuard } from "./guards/reportes/estadistica/kilos-comprados-por-proveedor.guards";
import { VentasMostradorVsRepartoGuard } from "./guards/reportes/estadistica/ventas-mostrador-vs-reparto.guards";
import { VentasPorListsaDePreciosGuard } from "./guards/reportes/estadistica/ventas-por-lista-de-precios.guards";
import { ReportesEstadisticasComprasGuard } from "./guards/reportes/estadistica/reportes-estadisticas-compras.guards";
import { ReportesEstadisticasGananciasGuard } from "./guards/reportes/estadistica/reportes-estadisticas-ganancias.guards";
import { VentasPorProductoVendedorGuard } from "./guards/reportes/listados/ventas-por-producto-vendedor.guards";
import { ClientesQueNoCompranDesdeFechaGuard } from "./guards/reportes/listados/clientes-que-no-compran-desde-fecha.guards";
import { ClienteCuentaCorrienteNewComponent } from "./views/clientes/cta_cte/new/cliente-cta-cte-new.component";
import { ClienteCuentaCorrienteComponent } from "./views/clientes/cta_cte/cliente-cta-cte.component";
import { ClienteCtaCteVerGuard } from "./guards/clientes/clientes.cta-cte-ver.guards";
import { ABMZonasGuard } from "./guards/zonas/abm.zonas.guards";
import { ZonasComponent } from "./views/zonas/zonas.component";
import { ZonaCrudComponent } from "./views/zonas/crud/zona-crud.component";
import { ABMTransportesGuard } from "./guards/transportes/abm.transportes.guards";
import { TransportesComponent } from "./views/transportes/transportes.component";
import { TransporteCrudComponent } from "./views/transportes/crud/transporte-crud.component";
import { ClienteCtaCteNuevoGuard } from "./guards/clientes/clientes.cta-cte-nuevo.guards";
import { ProveedoresCtaCteVerGuard } from "./guards/proveedores/proveedores.cta-cte-ver.guards";
import { ProveedoresCtaCteNuevoGuard } from "./guards/proveedores/proveedores.cta-cte-nuevo.guards";
import { ProveedorCuentaCorrienteComponent } from "./views/proveedores/cta_cte/proveedor-cta-cte.component";
import { ProveedorCuentaCorrienteNewComponent } from "./views/proveedores/cta_cte/new/proveedor-cta-cte-new.component";

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent, pathMatch: 'full' },
    { canActivate: [ AuthGuard ], path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'error-page', component: ErrorPageComponent, data: { message: "Se ha producido un error" } },
    { path: 'forbidden', component: ErrorPageComponent, data: { message: "No tienes permisos" } },
    { path: 'not-found', component: ErrorPageComponent, data: { message: "No se encontró la ruta solicitada" } },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: 'users', component: UsersComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/new", component: UserCrudComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/edit/:id", component: UserCrudComponent },
    { canActivate: [ AuthGuard ], path: "users/confirm/:data", component: UserConfirmComponent },
    { canActivate: [ AuthGuard ], path: "me/profile", component: MeProfileComponent },
    { canActivate: [ AuthGuard, ABMMarcasGuard ], path: 'marcas', component: MarcasComponent },
    { canActivate: [ AuthGuard, ABMMarcasGuard ], path: "marcas/new", component: MarcaCrudComponent },
    { canActivate: [ AuthGuard, ABMMarcasGuard ], path: "marcas/edit/:id", component: MarcaCrudComponent },
    { canActivate: [ AuthGuard, ABMZonasGuard ], path: 'zonas', component: ZonasComponent },
    { canActivate: [ AuthGuard, ABMZonasGuard ], path: "zonas/new", component: ZonaCrudComponent },
    { canActivate: [ AuthGuard, ABMZonasGuard ], path: "zonas/edit/:id", component: ZonaCrudComponent },
    { canActivate: [ AuthGuard, ABMTransportesGuard ], path: 'transportes', component: TransportesComponent },
    { canActivate: [ AuthGuard, ABMTransportesGuard ], path: "transportes/new", component: TransporteCrudComponent },
    { canActivate: [ AuthGuard, ABMTransportesGuard ], path: "transportes/edit/:id", component: TransporteCrudComponent },
    { canActivate: [ AuthGuard, CajaDiariaVerGuard ], path: 'cajas/diaria', component: CajaDiariaComponent },
    { canActivate: [ AuthGuard, CajaDiariaNuevoMovimientoGuard ], path: "cajas/diaria/new", component: CajaDiariaNewComponent },
    { canActivate: [ AuthGuard, CajaFuerteVerGuard ], path: 'cajas/fuerte', component: CajaFuerteComponent },
    { canActivate: [ AuthGuard, CajaFuerteNuevoMovimientoGuard ], path: "cajas/fuerte/new", component: CajaFuerteNewComponent },
    { canActivate: [ AuthGuard, CajaTransferenciaGuard ], path: "cajas/transferencia", component: CajaTransferenciaComponent },
    { canActivate: [ AuthGuard, VerClientesGuard ], path: 'clientes', component: ClientesComponent },
    { canActivate: [ AuthGuard, CRUDClientesGuard ], path: "clientes/new", component: ClienteCrudComponent },
    { canActivate: [ AuthGuard, CRUDClientesGuard ], path: "clientes/edit/:id", component: ClienteCrudComponent },
    { canActivate: [ AuthGuard, ClienteCtaCteVerGuard ], path: 'clientes/cta_cte/:id', component: ClienteCuentaCorrienteComponent },
    { canActivate: [ AuthGuard, ClienteCtaCteNuevoGuard ], path: 'clientes/cta_cte/:id/new', component: ClienteCuentaCorrienteNewComponent },
    { canActivate: [ AuthGuard, CRUDClientesGuard ], path: "clientes/new", component: ClienteCrudComponent },
    { canActivate: [ AuthGuard, VerProveedoresGuard ], path: 'proveedores', component: ProveedoresComponent },
    { canActivate: [ AuthGuard, CRUDProveedoresGuard ], path: "proveedores/new", component: ProveedorCrudComponent },
    { canActivate: [ AuthGuard, CRUDProveedoresGuard ], path: "proveedores/edit/:id", component: ProveedorCrudComponent },
    { canActivate: [ AuthGuard, ProveedoresCtaCteVerGuard ], path: 'proveedores/cta_cte/:id', component: ProveedorCuentaCorrienteComponent },
    { canActivate: [ AuthGuard, ProveedoresCtaCteNuevoGuard ], path: 'proveedores/cta_cte/:id/new', component: ProveedorCuentaCorrienteNewComponent },
    { canActivate: [ AuthGuard, VerProductosGuard ], path: 'productos', component: ProductosComponent },
    { canActivate: [ AuthGuard, CRUDProductosGuard ], path: "productos/new", component: ProductoCrudComponent },
    { canActivate: [ AuthGuard, CRUDProductosGuard ], path: "productos/edit/:id", component: ProductoCrudComponent },
    { canActivate: [ AuthGuard, VerPreciosGuard ], path: "precios", component: PreciosComponent },
    { canActivate: [ AuthGuard, CRUDPreciosGuard ], path: "precios/new", component: PrecioCrudComponent },
    { canActivate: [ AuthGuard, CRUDPreciosGuard ], path: "precios/renew/:id", component: PrecioCrudComponent },
    { canActivate: [ AuthGuard, ReajustarPreciosGuard ], path: "precios/reajustes", component: PreciosReajustesComponent },
    { canActivate: [ AuthGuard, ReajustarPreciosGuard ], path: "precios/reajustes/new", component: PrecioReajusteCrudComponent },
    { canActivate: [ AuthGuard, VerStockGuard ], path: "stock", component: StockComponent },
    { canActivate: [ AuthGuard, NuevoMovimientoStockGuard ], path: "stock/new", component: StockNewComponent },
    { canActivate: [ AuthGuard, VerPedidosGuard ], path: "pedidos", component: PedidosComponent },
    { canActivate: [ AuthGuard, NuevoPedidoGuard ], path: "pedidos/new", component: PedidoCrudComponent },
    { canActivate: [ AuthGuard, VerVentasGuard ], path: "ventas", component: VentasComponent },
    { canActivate: [ AuthGuard, NuevoVentasGuard ], path: "ventas/new", component: VentaCrudComponent },
    { canActivate: [ AuthGuard, VentasPorProductoVendedorGuard ], path: "reportes/listados/ventas-por-producto-vendedor", component: ReportesListadosVentasPorProductoProveedorComponent },
    { canActivate: [ AuthGuard, ClientesQueNoCompranDesdeFechaGuard ], path: "reportes/listados/clientes-que-no-compran-desde-fecha", component: ClientesQueNoCompranDesdeFechaComponent },
    { canActivate: [ AuthGuard, KilosCompradosPorProveedorGuard ], path: "reportes/estadistica/kilos-comprados-por-cada-proveedor", component: KilosCompradosPorCadaProveedorComponent },
    { canActivate: [ AuthGuard, VentasMostradorVsRepartoGuard ], path: "reportes/estadistica/ventas-reparto-vs-mostrador", component: VentasRepartoVsMostradorComponent },
    { canActivate: [ AuthGuard, VentasPorListsaDePreciosGuard ], path: "reportes/estadistica/total-ventas-por-lista-de-precios", component: TotalVentasPorListaDePreciosComponent },
    { canActivate: [ AuthGuard, ReportesEstadisticasComprasGuard ], path: "reportes/estadistica/compras", component: ReportesEstadisticaComprasComponent },
    { canActivate: [ AuthGuard, ReportesEstadisticasGananciasGuard ], path: "reportes/estadistica/ganancias", component: ReportesEstadisticaGananciasComponent }
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [ RouterModule ]
})
export class AppRoutingModule {

}