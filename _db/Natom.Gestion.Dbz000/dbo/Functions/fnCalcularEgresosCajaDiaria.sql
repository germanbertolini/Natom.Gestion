
CREATE FUNCTION fnCalcularEgresosCajaDiaria
(
	@Fecha DATE
)
RETURNS DECIMAL(18,2)
AS
BEGIN

	DECLARE @Return DECIMAL(18,2) = 0;

	SELECT
		@Return = SUM(Importe)
	FROM
		MovimientoCajaDiaria M WITH(NOLOCK)
	WHERE
		M.Tipo = 'D'
		AND M.Observaciones NOT LIKE 'TRANSFERENCIA%' --DEBITOS QUE NO SON TRANSFERENCIA, ES DECIR, GASTOS!
		AND CAST(M.FechaHora AS DATE) = @Fecha

	RETURN COALESCE(@Return, 0)

END