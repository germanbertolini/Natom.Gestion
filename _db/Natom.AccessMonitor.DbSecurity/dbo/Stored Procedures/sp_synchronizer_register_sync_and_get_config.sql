CREATE PROCEDURE [dbo].[sp_synchronizer_register_sync_and_get_config]
	@InstanceId CHAR(32),
	@CurrentSyncToServerMinutes INT,
	@CurrentSyncFromDevicesMinutes INT
AS
BEGIN

	
	SELECT
		NewSyncToServerMinutes,
		NewSyncFromDevicesMinutes
	FROM
		[dbo].[Synchronizer] WITH(NOLOCK)
	WHERE
		InstanceId = @InstanceId;

	UPDATE
		[dbo].[Synchronizer] WITH(READPAST)
	SET
		LastSyncAt = GETDATE(),
		LastConnectionAt = GETDATE(),
		CurrentSyncToServerMinutes = COALESCE(NewSyncToServerMinutes, @CurrentSyncToServerMinutes, CurrentSyncToServerMinutes),
		CurrentSyncFromDevicesMinutes = COALESCE(NewSyncFromDevicesMinutes, @CurrentSyncFromDevicesMinutes, CurrentSyncFromDevicesMinutes),
		NewSyncToServerMinutes = NULL,
		NewSyncFromDevicesMinutes = NULL
	WHERE
		InstanceId = @InstanceId;

END