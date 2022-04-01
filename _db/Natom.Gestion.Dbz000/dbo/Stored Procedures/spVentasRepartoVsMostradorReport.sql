

CREATE PROCEDURE spVentasRepartoVsMostradorReport
(
	@Desde DATE
)
AS
BEGIN

	SELECT
		J.Codigo,
		J.Descripcion,
		J.CantidadMostrador,
		J.VariacionCantidadMostrador,
		CASE WHEN J.VariacionCantidadMostrador < 0 THEN 'Red'
		WHEN J.VariacionCantidadMostrador = 0 THEN 'Black'
		ELSE 'Green' END AS ColorVariacionCantidadMostrador,
		J.KilosMostrador,
		J.VariacionKilosMostrador,
		CASE WHEN J.VariacionKilosMostrador < 0 THEN 'Red'
		WHEN J.VariacionKilosMostrador = 0 THEN 'Black'
		ELSE 'Green' END AS ColorVariacionKilosMostrador,
		J.CantidadReparto,
		J.VariacionCantidadReparto,
		CASE WHEN J.VariacionCantidadReparto < 0 THEN 'Red'
		WHEN J.VariacionCantidadReparto = 0 THEN 'Black'
		ELSE 'Green' END AS ColorVariacionCantidadReparto,
		J.KilosReparto,
		J.VariacionKilosReparto,
		CASE WHEN J.VariacionKilosReparto < 0 THEN 'Red'
		WHEN J.VariacionKilosReparto = 0 THEN 'Black'
		ELSE 'Green' END AS ColorVariacionKilosReparto
	FROM
	(
		SELECT
			R.Codigo,
			R.DescripcionCorta AS Descripcion,
			R.CantidadMostrador,
			[dbo].[fnCalcularVariacion] (CAST(R.CantidadMostrador AS DECIMAL(18,2)), CAST(R.CantidadReparto AS decimal(18,2))) AS VariacionCantidadMostrador,
			R.KilosMostrador,
			[dbo].[fnCalcularVariacion] (CAST(R.KilosMostrador AS DECIMAL(18,2)), CAST(R.KilosReparto AS decimal(18,2))) AS VariacionKilosMostrador,

			R.CantidadReparto,		
			[dbo].[fnCalcularVariacion] (CAST(R.CantidadReparto AS DECIMAL(18,2)), CAST(R.CantidadMostrador AS decimal(18,2))) AS VariacionCantidadReparto,
			R.KilosReparto,
			[dbo].[fnCalcularVariacion] (CAST(R.KilosReparto AS DECIMAL(18,2)), CAST(R.KilosMostrador AS decimal(18,2))) AS VariacionKilosReparto
		FROM
		(
			SELECT
				P.Codigo,
				P.DescripcionCorta,
				SUM(CASE WHEN OP.OrdenDePedidoId IS NULL THEN COALESCE(OPD.Cantidad, D.Cantidad) ELSE 0 END) AS CantidadMostrador,
				SUM(CASE WHEN OP.OrdenDePedidoId IS NOT NULL THEN COALESCE(OPD.Cantidad, D.Cantidad) ELSE 0 END) AS CantidadReparto,
				SUM(CASE WHEN OP.OrdenDePedidoId IS NULL THEN COALESCE((OPD.Cantidad * CAST(OPD.PesoUnitarioEnGramos AS DECIMAL(18,2)) / 1000), (D.Cantidad * CAST(D.PesoUnitarioEnGramos AS DECIMAL(18,2)) / 1000)) ELSE 0 END) AS KilosMostrador,
				SUM(CASE WHEN OP.OrdenDePedidoId IS NOT NULL THEN COALESCE((OPD.Cantidad * CAST(OPD.PesoUnitarioEnGramos AS DECIMAL(18,2)) / 1000), (D.Cantidad * CAST(D.PesoUnitarioEnGramos AS DECIMAL(18,2)) / 1000)) ELSE 0 END) AS KilosReparto
			FROM
				VentaDetalle D WITH(NOLOCK)
				INNER JOIN Venta V WITH(NOLOCK) ON V.VentaId = D.VentaId AND V.Activo = 1
				INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = D.ProductoId
				LEFT JOIN OrdenDePedidoDetalle OPD WITH(NOLOCK) ON OPD.OrdenDePedidoDetalleId = D.OrdenDePedidoDetalleId
				LEFT JOIN OrdenDePedido OP WITH(NOLOCK) ON OP.OrdenDePedidoId = OPD.OrdenDePedidoDetalleId AND OP.Activo = 1
			WHERE
				@Desde IS NULL
				OR (@Desde IS NOT NULL AND COALESCE(OP.FechaHoraPedido, V.FechaHoraVenta) >= @Desde)
			GROUP BY
				P.ProductoId, P.Codigo, P.DescripcionCorta
		) AS R
	) AS J
	   
END