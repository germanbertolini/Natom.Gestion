
CREATE PROCEDURE spTotalVendidoPorListaDePreciosReport
(
	@Desde DATE
)
AS
BEGIN

	SELECT
		COALESCE(L.Descripcion, '-Sin lista de precios-') AS ListaDePrecios,
		SUM(D.Precio * D.Cantidad)  AS MontoVendido,
		SUM(CAST(D.PesoUnitarioEnGramos AS DECIMAL(18,2)) / 1000 * D.Cantidad)  AS KilosVendidos
	FROM
		VentaDetalle D WITH(NOLOCK)
		INNER JOIN Venta V WITH(NOLOCK) ON V.VentaId = D.VentaId AND V.Activo = 1
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = D.ProductoId
		LEFT JOIN ListaDePrecios L WITH(NOLOCK) ON L.ListaDePreciosId = D.ListaDePreciosId
	WHERE
		@Desde IS NULL
		OR (@Desde IS NOT NULL AND V.FechaHoraVenta >= @Desde)
	GROUP BY
		L.ListaDePreciosId, L.Descripcion
			   
END