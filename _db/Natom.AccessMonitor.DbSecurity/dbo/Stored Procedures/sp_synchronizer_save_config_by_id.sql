CREATE PROCEDURE [dbo].[sp_synchronizer_save_config_by_id]
	@InstanceId NVARCHAR(32),
	@IntervalMinsFromDevice INT,
	@IntervalMinsToServer INT
AS
BEGIN

	UPDATE [dbo].[Synchronizer] WITH(READPAST)
		SET NewSyncToServerMinutes = @IntervalMinsToServer,
			NewSyncFromDevicesMinutes = @IntervalMinsFromDevice
		WHERE
			InstanceId = @InstanceId

END