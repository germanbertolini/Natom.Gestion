CREATE PROCEDURE [dbo].[sp_device_list_unassigned_by_clientid]
	@ClientId INT
AS
BEGIN

	SELECT
		D.DeviceId,
		D.DeviceName
	FROM
		[dbo].[Device] D WITH(NOLOCK)
		INNER JOIN [dbo].[Synchronizer] S WITH(NOLOCK) ON S.InstanceId = D.InstanceId
	WHERE
		D.GoalId IS NULL
		AND S.RemovedAt IS NULL
		AND S.ClientId = @ClientId

END