
CREATE PROCEDURE spVentasPorProductoProveedorReport
(
	@ProductoId INT,
	@ProveedorId INT,
	@Desde DATE,
	@Hasta DATE
)
AS
BEGIN

	SELECT
		COALESCE(OP.FechaHoraPedido, V.FechaHoraVenta) AS FechaHora,
		CASE WHEN OP.OrdenDePedidoId IS NULL THEN 'Mostrador' ELSE 'Reparto' END AS VendidoPor,
		CASE WHEN OP.OrdenDePedidoId IS NULL THEN 'VTA ' + REPLACE(STR(V.NumeroVenta, 8), SPACE(1), '0') ELSE 'OP ' + REPLACE(STR(OP.NumeroPedido, 8), SPACE(1), '0') END AS Operacion,
		'RTO ' + OP.NumeroRemito AS Remito,
		REPLACE(STR(V.NumeroVenta, 8), SPACE(1), '0') AS Venta,
		V.TipoFactura + '-' + V.NumeroFactura AS ComprobanteVenta,
		CASE WHEN PR.EsEmpresa = 1 THEN PR.RazonSocial ELSE PR.Nombre + ' ' + PR.Apellido END AS Proveedor,
		CONCAT(P.Codigo, ' ', P.DescripcionCorta) AS Producto,
		D.Cantidad,
		(CAST(D.PesoUnitarioEnGramos AS decimal(18,2)) / 1000) * D.Cantidad AS PesoTotalKilos,
		D.Precio * D.Cantidad AS MontoTotal
	FROM
		VentaDetalle D WITH(NOLOCK)
		INNER JOIN Venta V WITH(NOLOCK) ON V.VentaId = D.VentaId AND V.Activo = 1
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = D.ProductoId
		LEFT JOIN Proveedor PR WITH(NOLOCK) ON PR.ProveedorId = P.ProveedorId
		LEFT JOIN OrdenDePedidoDetalle OPD WITH(NOLOCK) ON OPD.OrdenDePedidoDetalleId = D.OrdenDePedidoDetalleId
		LEFT JOIN OrdenDePedido OP WITH(NOLOCK) ON OP.OrdenDePedidoId = OPD.OrdenDePedidoDetalleId AND OP.Activo = 1
	WHERE
		P.ProductoId = COALESCE(@ProductoId, P.ProductoId)
		AND P.ProveedorId = COALESCE(@ProveedorId, P.ProveedorId)
		AND COALESCE(OP.FechaHoraPedido, V.FechaHoraVenta) >= @Desde AND COALESCE(OP.FechaHoraPedido, V.FechaHoraVenta) <= @Hasta
	ORDER BY
		P.Codigo, P.DescripcionCorta, PR.RazonSocial, PR.Nombre, PR.Apellido

END