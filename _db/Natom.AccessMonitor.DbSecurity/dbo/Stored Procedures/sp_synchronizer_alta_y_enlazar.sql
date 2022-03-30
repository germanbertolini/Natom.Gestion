CREATE PROCEDURE [dbo].[sp_synchronizer_alta_y_enlazar]
(
	@InstanceId NVARCHAR(32),
	@ClienteId INT
)
AS
BEGIN

	UPDATE [dbo].[Synchronizer]
	SET ActivatedAt = GETDATE(),
		ClientId = @ClienteId,
		RemovedAt = NULL
	WHERE
		InstanceId = @InstanceId

END