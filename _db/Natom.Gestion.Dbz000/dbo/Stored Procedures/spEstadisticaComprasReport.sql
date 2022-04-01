
CREATE PROCEDURE spEstadisticaComprasReport
(
	@Desde DATE,
	@Hasta DATE
)
AS
BEGIN

	IF @Hasta IS NULL
		SET @Hasta = '2099-01-01';

	SELECT
		COALESCE(SUM(CASE WHEN P.EsPresupuesto = 1 THEN M.CostoUnitario * M.Cantidad ELSE 0 END), 0) AS TotalPresupuesto,
		COALESCE(SUM(CASE WHEN P.EsPresupuesto = 0 THEN M.CostoUnitario * M.Cantidad ELSE 0 END), 0) AS TotalCompras
	FROM
		MovimientoStock M WITH(NOLOCK)
		INNER JOIN Proveedor P WITH(NOLOCK) ON P.ProveedorId = M.ProveedorId
		INNER JOIN Producto PR WITH(NOLOCK) ON PR.ProductoId = M.ProductoId
	WHERE
		M.EsCompra = 1
		AND M.FechaHora >= @Desde AND M.FechaHora <= @Hasta
			   
END