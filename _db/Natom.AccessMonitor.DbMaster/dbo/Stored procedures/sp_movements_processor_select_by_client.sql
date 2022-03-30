CREATE PROCEDURE [dbo].[sp_movements_processor_select_by_client]
(
	@ClientId INT
)
AS
BEGIN

	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		SELECT TOP (1000)
			M.MovementId,
			M.DeviceId,
			M.[DateTime],
			M.DocketNumber,
			M.MovementType,
			M.GoalId,
			G.PlaceId,
			D.DocketId,
			D.PlaceId AS ExpectedPlaceId
		INTO
			#TMP_MOVIMIENTOS
		FROM
			[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '] M WITH(NOLOCK)
			INNER JOIN [dbo].[Goal] G WITH(NOLOCK) ON G.GoalId = M.[GoalId]
			INNER JOIN [dbo].[Place] P WITH(NOLOCK) ON P.PlaceId = G.PlaceId
			INNER JOIN [dbo].[Docket] D WITH(NOLOCK) ON D.DocketNumber = M.DocketNumber AND D.Active = 1 AND D.ClientId = P.ClientId
		WHERE
			M.[GoalId] IS NOT NULL
			AND M.[ProcessedAt] IS NULL;


		SELECT
			*
		INTO
			#TMP_DOCKET_RANGES
		FROM
			[dbo].[DocketRange] WITH(NOLOCK)
		WHERE
			DocketId IN (SELECT DocketId FROM #TMP_MOVIMIENTOS)
			AND [From] IS NOT NULL
			AND [To] IS NOT NULL;


		SELECT
			*
		FROM
			#TMP_MOVIMIENTOS
		WHERE
			DocketId IN (SELECT DocketId FROM #TMP_DOCKET_RANGES);


		SELECT
			*
		FROM
			#TMP_DOCKET_RANGES;



		SELECT
			P.*
		FROM
			[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] P WITH(NOLOCK)
			INNER JOIN
			(
				SELECT
					DocketId,
					MAX(DATEADD(ms, DATEDIFF(ms, ''00:00:00'', [ExpectedIn]), CONVERT(DATETIME, [Date]))) AS LastDateTimeIn
				FROM
					[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] WITH(NOLOCK)
				WHERE
					[Date] > DATEADD(MONTH, -4, GETDATE())
				GROUP BY DocketId
			) AS LastProcess ON LastProcess.DocketId = P.DocketId AND CAST(LastProcess.LastDateTimeIn AS TIME) = P.[ExpectedIn] AND CAST(LastProcess.LastDateTimeIn AS DATE) = P.[Date]

	';

	EXEC sp_executesql @SQL

END