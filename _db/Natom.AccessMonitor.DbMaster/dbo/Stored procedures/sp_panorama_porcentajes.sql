CREATE PROCEDURE [dbo].[sp_panorama_porcentajes]
(
	@ClientId INT,
	@PlaceId INT = NULL
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		DECLARE @Hoy DATE = CAST(GETDATE() AS DATE)
		DECLARE @SieteDias DATE = DATEADD(DAY, -7, @Hoy);
		DECLARE @QuinceDias DATE = DATEADD(DAY, -15, @Hoy);
		DECLARE @TreintaDias DATE = DATEADD(DAY, -30, @Hoy);

		SELECT
			7 AS Modalidad,
			COUNT(*) AS CantidadTotal,
			COALESCE(SUM(CASE WHEN [In] IS NOT NULL THEN 1 ELSE 0 END), 0) AS CantidadPresentes
		FROM
			[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] AS P WITH(NOLOCK)
		WHERE
			[Date] < @Hoy AND [Date] >= @SieteDias
			AND ExpectedPlaceId = COALESCE(' + (CASE WHEN @PlaceId IS NULL THEN 'NULL' ELSE CAST(@PlaceId AS VARCHAR) END) + ', ExpectedPlaceId)

		UNION ALL

		SELECT
			15 AS Modalidad,
			COUNT(*) AS CantidadTotal,
			COALESCE(SUM(CASE WHEN [In] IS NOT NULL THEN 1 ELSE 0 END), 0) AS CantidadPresentes
		FROM
			[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] AS P WITH(NOLOCK)
		WHERE
			[Date] < @Hoy AND [Date] >= @QuinceDias
			AND ExpectedPlaceId = COALESCE(' + (CASE WHEN @PlaceId IS NULL THEN 'NULL' ELSE CAST(@PlaceId AS VARCHAR) END) + ', ExpectedPlaceId)

		UNION ALL

		SELECT
			30 AS Modalidad,
			COUNT(*) AS CantidadTotal,
			COALESCE(SUM(CASE WHEN [In] IS NOT NULL THEN 1 ELSE 0 END), 0) AS CantidadPresentes
		FROM
			[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] AS P WITH(NOLOCK)
		WHERE
			[Date] < @Hoy AND [Date] >= @TreintaDias
			AND ExpectedPlaceId = COALESCE(' + (CASE WHEN @PlaceId IS NULL THEN 'NULL' ELSE CAST(@PlaceId AS VARCHAR) END) + ', ExpectedPlaceId)

	';

	EXEC sp_executesql @SQL

END
