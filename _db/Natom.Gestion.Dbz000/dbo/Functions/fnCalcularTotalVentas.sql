
CREATE FUNCTION fnCalcularTotalVentas
(
	@Fecha DATE
)
RETURNS DECIMAL(18,2)
AS
BEGIN

	DECLARE @Return DECIMAL(18,2) = 0;

	SELECT
		@Return = SUM(MontoTotal)
	FROM
		Venta V WITH(NOLOCK)
	WHERE
		V.Activo = 1
		AND CAST(V.FechaHoraVenta AS DATE) = @Fecha

	RETURN COALESCE(@Return, 0)

END