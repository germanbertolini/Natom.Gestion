CREATE PROCEDURE [dbo].[sp_synchronizer_select_sync_times]
	@ClienteId INT
AS
BEGIN

	SELECT
		*,
		DATEDIFF(SECOND, GETDATE(), NextSyncAt) * 1000 AS NextOnMiliseconds
	FROM
	(
		SELECT
			InstanceId,
			InstallerName,
			LastSyncAt,
			DATEADD(MINUTE, CurrentSyncToServerMinutes, LastSyncAt) AS NextSyncAt,
			CurrentSyncToServerMinutes * 60 * 1000 AS EachMiliseconds
		FROM
			[dbo].[Synchronizer] WITH(NOLOCK)
		WHERE
			ClientId = @ClienteId
			AND RemovedAt IS NULL
			AND CurrentSyncToServerMinutes IS NOT NULL
	) R
	ORDER BY
		LastSyncAt DESC

END