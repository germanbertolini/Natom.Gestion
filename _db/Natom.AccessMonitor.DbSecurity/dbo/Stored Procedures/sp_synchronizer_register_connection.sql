CREATE PROCEDURE [dbo].[sp_synchronizer_register_connection]
	@InstanceId CHAR(32)
AS
BEGIN

	UPDATE
		[dbo].[Synchronizer] WITH(READPAST)
	SET
		LastConnectionAt = GETDATE()
	WHERE
		InstanceId = @InstanceId;

END