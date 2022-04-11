
CREATE FUNCTION fnCalcularStockAlMovimiento
(
	@MovimientoStockId INT,
	@DepositoId INT,
	@ProductoId INT
)
RETURNS DECIMAL(18,2)
AS
BEGIN

	DECLARE @Cantidad INT = 0;

	SELECT
		@Cantidad = SUM (CASE WHEN Tipo = 'E' THEN Cantidad * -1 ELSE Cantidad END)
	FROM
		MovimientoStock WITH(NOLOCK)
	WHERE
		MovimientoStockId <= @MovimientoStockId
		AND DepositoId = COALESCE(@DepositoId, DepositoId)
		AND ProductoId = @ProductoId;

	RETURN COALESCE(@Cantidad, 0);
END