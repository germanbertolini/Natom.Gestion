
CREATE FUNCTION fnCalcularTotalCostosVentas
(
	@Fecha DATE
)
RETURNS DECIMAL(18,2)
AS
BEGIN

	DECLARE @Return DECIMAL(18,2) = 0;

	SELECT
		@Return = SUM(P.CostoUnitario * D.Cantidad)
	FROM
		Venta V WITH(NOLOCK)
		INNER JOIN VentaDetalle D WITH(NOLOCK) ON D.VentaId = V.VentaId
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = D.ProductoId
	WHERE
		V.Activo = 1
		AND CAST(V.FechaHoraVenta AS DATE) = @Fecha

	RETURN COALESCE(@Return, 0)

END