CREATE PROCEDURE [dbo].[sp_synchronizer_baja_by_id]
(
	@SyncInstanceId NVARCHAR(32)
)
AS
BEGIN

	UPDATE [dbo].[Synchronizer]
		SET RemovedAt = GETDATE()
		WHERE InstanceId = @SyncInstanceId;

	SELECT
		*
	FROM
		[dbo].[Token] WITH(NOLOCK)
		WHERE SyncInstanceId = @SyncInstanceId
		AND (ExpiresAt IS NULL OR ExpiresAt > GETDATE());

END