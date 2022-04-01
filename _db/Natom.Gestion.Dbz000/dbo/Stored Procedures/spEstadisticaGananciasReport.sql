
CREATE PROCEDURE spEstadisticaGananciasReport
(
	@Desde DATE,
	@Hasta DATE
)
AS
BEGIN

	IF @Hasta IS NULL
		SET @Hasta = GETDATE()

	CREATE TABLE #dates(Fecha DATETIME)

	DECLARE @TDesde DATE = @Desde;

	WHILE @TDesde <= @Hasta
	BEGIN
		INSERT INTO #dates VALUES (@TDesde);
		SET @TDesde = DATEADD(DAY, 1, @TDesde)
	END  

	SELECT
		R.Fecha,
		R.TotalVentas,
		R.TotalCostosVentas,
		R.TotalEgresosCajaDiaria,
		R.TotalVentas - R.TotalCostosVentas - R.TotalEgresosCajaDiaria AS TotalGanancias
	FROM
	(
		SELECT
			Fecha,
			dbo.fnCalcularTotalVentas(Fecha) AS TotalVentas,
			dbo.fnCalcularTotalCostosVentas(Fecha) AS TotalCostosVentas,
			dbo.fnCalcularEgresosCajaDiaria(Fecha) AS TotalEgresosCajaDiaria
		FROM
			#dates
	) R

END