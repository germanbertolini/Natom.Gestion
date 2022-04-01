
CREATE VIEW vwClientesUltimaCompra
AS

	SELECT
		MAX(R.FechaHora) AS FechaHora,
		R.ClienteId
	FROM
	(
		SELECT
			MAX(COALESCE(OP.FechaHoraPedido, V.FechaHoraVenta)) AS FechaHora,
			V.ClienteId
		FROM
			Venta V WITH(NOLOCK)
			LEFT JOIN OrdenDePedido OP WITH(NOLOCK) ON OP.VentaId = V.VentaId
		WHERE
			V.Activo = 1
		GROUP BY
			V.ClienteId

		UNION

		SELECT
			MAX(OP.FechaHoraPedido) AS FechaHora,
			OP.ClienteId
		FROM
			OrdenDePedido OP WITH(NOLOCK)
		WHERE
			OP.Activo = 1
			AND OP.VentaId IS NULL
		GROUP BY
			OP.ClienteId
	) AS R
	GROUP BY
		R.ClienteId