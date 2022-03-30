CREATE PROCEDURE [dbo].[sp_panorama_actual]
(
	@ClientId INT,
	@PlaceId INT = NULL
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		SELECT
			R.CantidadTotal,
			COALESCE(R.CantidadPresentes, 0) AS CantidadPresentes,
			COALESCE(R.CantidadAusentes, 0) AS CantidadAusentes,
			COALESCE(CAST((R.CantidadPresentes * 100 / R.CantidadTotal) AS int), 0) AS PorcentajeAsistencia
		FROM
		(
			SELECT
				COUNT(*) AS CantidadTotal,
				SUM(CASE WHEN P.[In] IS NOT NULL THEN 1 ELSE 0 END) AS CantidadPresentes,
				SUM(CASE WHEN P.[In] IS NULL AND DATEADD(MINUTE, 30, T.[From]) < GETDATE() THEN 1 ELSE 0 END) AS CantidadAusentes
			FROM
				[dbo].[fn_current_turnos_acumulados] (' + CAST(@ClientId AS VARCHAR) + ', ' + (CASE WHEN @PlaceId IS NULL THEN 'NULL' ELSE CAST(@PlaceId AS VARCHAR) END) + ') AS T
				LEFT JOIN [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] AS P WITH(NOLOCK) ON T.DocketId = P.DocketId AND T.[Date] = P.[Date] AND CAST(T.[From] AS TIME) = P.ExpectedIn
			WHERE
				T.[Date] = CAST(GETDATE() AS date)
		) R
	';

	EXEC sp_executesql @SQL

END