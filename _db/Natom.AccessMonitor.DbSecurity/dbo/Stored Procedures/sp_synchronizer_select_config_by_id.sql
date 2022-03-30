CREATE PROCEDURE [dbo].[sp_synchronizer_select_config_by_id]
	@InstanceId NVARCHAR(32)
AS
BEGIN

	SELECT
		InstanceId,
		COALESCE(NewSyncToServerMinutes, CurrentSyncToServerMinutes) AS SyncToServerMinutes,
		COALESCE(NewSyncFromDevicesMinutes, CurrentSyncFromDevicesMinutes) AS SyncFromDevicesMinutes,
		LastSyncAt,
		CASE WHEN DATEADD(MINUTE, COALESCE(NewSyncToServerMinutes, CurrentSyncToServerMinutes), LastSyncAt) < DATEADD(MINUTE, -5, GETDATE()) THEN
			NULL
		ELSE
			DATEADD(MINUTE, COALESCE(NewSyncToServerMinutes, CurrentSyncToServerMinutes), LastSyncAt)
		END AS NextSyncAt
	FROM
		[dbo].[Synchronizer] WITH(NOLOCK)
	WHERE
		InstanceId = @InstanceId

END