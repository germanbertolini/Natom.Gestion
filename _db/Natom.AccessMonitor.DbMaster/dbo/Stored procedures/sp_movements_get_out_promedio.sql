CREATE PROCEDURE [dbo].[sp_movements_get_out_promedio]
(
	@ClientId INT,
	@DocketId INT
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		SELECT
			cast(cast(avg(cast(CAST(convert(varchar, [Out], 108) as datetime) as float)) as datetime) as time) AS OutPromedio
		FROM
		(
			SELECT TOP(10)
				*
			FROM
				[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed]
			WHERE
				[DocketId] = ' + CAST(@DocketId AS varchar) + '
				AND [Out] IS NOT NULL
				AND OutWasEstimated = 0
			ORDER BY
				[Date] DESC
		) AS R';

	EXEC sp_executesql @SQL

END