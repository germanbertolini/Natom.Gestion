--dbo.Permiso---------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([PermisoId],[Scope],[Descripcion])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('abm_usuarios','WebApp.Admin','ABM SysAdmins'),
('abm_clientes','WebApp.Admin','ABM Clientes'),
('abm_clientes_usuarios', 'WebApp.Admin', 'ABM Usuarios de clientes'),

('*','WebApp.Clientes','Permiso total'),
('ABM_MARCAS','WebApp.Clientes','ABM Marcas'),
('ABM_USUARIOS','WebApp.Clientes','ABM Usuarios'),
('CAJA_DIARIA_NUEVO_MOVIMIENTO','WebApp.Clientes','Caja diaria: Registrar movimientos'),
('CAJA_DIARIA_VER','WebApp.Clientes','Caja diaria: Ver movimientos'),
('CAJA_FUERTE_NUEVO_MOVIMIENTO','WebApp.Clientes','Tesorería: Registrar movimientos'),
('CAJA_FUERTE_VER','WebApp.Clientes','Tesorería: Ver movimientos'),
('CAJA_TRANSFERENCIA','WebApp.Clientes','Transferencia entre cajas'),
('CLIENTES_CRUD','WebApp.Clientes','Clientes: Alta, Baja, Modificación'),
('CLIENTES_CTA_CTE_NUEVO','WebApp.Clientes','Clientes: Nuevo movimiento en Cuenta Corriente'),
('CLIENTES_CTA_CTE_VER','WebApp.Clientes','Clientes: Consultar Cuenta Corriente'),
('CLIENTES_VER','WebApp.Clientes','Clientes: Consultar'),
('PEDIDOS_ANULAR','WebApp.Clientes','Pedidos: Anular pedido'),
('PEDIDOS_DEPOSITO','WebApp.Clientes','Pedidos: Armado / Finalización de preparación'),
('PEDIDOS_NUEVO','WebApp.Clientes','Pedidos: Carga nuevo pedido'),
('PEDIDOS_VER','WebApp.Clientes','Pedidos: Ver pedidos'),
('PEDIDOS_RANGOS_HORARIOS_CRUD','WebApp.Clientes','Rangos horarios de entrega: Alta, Baja, Modificación'),
('PRECIOS_CRUD','WebApp.Clientes','Precios: Alta, Baja, Modificación'),
('PRECIOS_REAJUSTAR','WebApp.Clientes','Precios: Reajuste por Marca'),
('PRECIOS_VER','WebApp.Clientes','Precios: Consultar'),
('PRECIOS_LISTAS_CRUD','WebApp.Clientes','Listas de precios: Alta, Baja, Modificación'),
('PRODUCTOS_CRUD','WebApp.Clientes','Productos: Alta, Baja, Modificación'),
('PRODUCTOS_VER','WebApp.Clientes','Productos: Consultar'),
('PRODUCTOS_CATEGORIAS_CRUD','WebApp.Clientes','Categorías de producto: Alta, Baja, Modificación'),
('PROVEEDORES_CRUD','WebApp.Clientes','Proveedores: Alta, Baja, Modificación'),
('PROVEEDORES_CTA_CTE_NUEVO','WebApp.Clientes','Proveedores: Nuevo movimiento en Cuenta Corriente'),
('PROVEEDORES_CTA_CTE_VER','WebApp.Clientes','Proveedores: Consultar Cuenta Corriente'),
('PROVEEDORES_VER','WebApp.Clientes','Proveedores: Consultar'),
('REPORTES_COMPRAS','WebApp.Clientes','Reporte: Compras'),
('REPORTES_ESTADISTICA_KILOS_COMPRADOS_POR_PROVEEDOR','WebApp.Clientes','Reporte: Estadística de kilos comprados por cada proveedor'),
('REPORTES_ESTADISTICA_VENTAS_POR_LISTA_DE_PRECIOS','WebApp.Clientes','Reporte: Estadística de cuánto se vendió por cada lista de precios'),
('REPORTES_ESTADISTICA_VENTAS_REPARTO_MOSTRADOR','WebApp.Clientes','Reporte: Estadística de cuánto vendió reparto vs mostrador'),
('REPORTES_GANANCIAS','WebApp.Clientes','Reporte: Ganancias'),
('REPORTES_LISTADO_CLIENTES_QUE_NO_COMPRAN','WebApp.Clientes','Reporte: Listado de clientes que no compran desde una cierta fecha'),
('REPORTES_LISTADO_VENTAS_POR_PRODUCTO','WebApp.Clientes','Reporte: Listado de ventas por producto y/o proveedor'),
('STOCK_CONTROL','WebApp.Clientes','Stock: Conteo y control'),
('STOCK_NUEVO_MOVIMIENTO','WebApp.Clientes','Stock: Registrar movimientos'),
('STOCK_VER','WebApp.Clientes','Stock: Ver movimientos'),
('TRANSPORTES_CRUD','WebApp.Clientes','Transportes: Alta, Baja, Modificación'),
('VENTAS_ANULAR','WebApp.Clientes','Ventas: Anular venta'),
('VENTAS_NUEVO','WebApp.Clientes','Ventas: Carga nueva venta'),
('VENTAS_VER','WebApp.Clientes','Ventas: Ver ventas'),
('ZONAS_CRUD','WebApp.Clientes','Zonas: Alta, Baja, Modificación'),
('DEPOSITOS_CRUD','WebApp.Clientes','Depósitos: Alta, Baja, Modificación')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([PermisoId],[Scope],[Descripcion]))
merge	[dbo].[Permiso] as t
using	cte_data as s
on		1=1 and t.[PermisoId] = s.[PermisoId] and t.[Scope] = s.[Scope]
when matched then
	update set
	[Descripcion] = s.[Descripcion]
when not matched by target then
	insert([PermisoId],[Scope],[Descripcion])
	values(s.[PermisoId],s.[Scope],s.[Descripcion])
when not matched by source then
	delete;