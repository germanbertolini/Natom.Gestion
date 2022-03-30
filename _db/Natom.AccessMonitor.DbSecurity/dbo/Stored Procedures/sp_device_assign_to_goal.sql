CREATE PROCEDURE [dbo].[sp_device_assign_to_goal]
	@Id INT,
	@GoalId INT
AS
BEGIN

	DECLARE @InstanceId NVARCHAR(32);
	DECLARE @DeviceId INT;
	DECLARE @ClientId INT;

	SELECT
		@InstanceId = InstanceId,
		@DeviceId = DeviceId
	FROM
		[dbo].[Device] WITH(NOLOCK)
	WHERE
		Id = @Id;


	SELECT
		@ClientId = P.ClientId
	FROM
		[$(DbMaster)].[dbo].[Goal] G WITH(NOLOCK)
		INNER JOIN [$(DbMaster)].[dbo].[Place] P WITH(NOLOCK) ON P.PlaceId = G.PlaceId
	WHERE
		G.GoalId = @GoalId;


	--PRIMERO ACTUALIZAMOS TODOS LOS MOVIMIENTOS SIN GOALID, QUE ES LO MAS COSTOSO
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		UPDATE [$(DbMaster)].[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '] WITH(READPAST)
			SET GoalId = ' + CAST(@GoalId AS VARCHAR) + ' 
			WHERE InstanceId = ''' + @InstanceId + ''' 
				AND DeviceId = ' + CAST(@DeviceId AS VARCHAR) + ' 
				AND GoalId IS NULL;
	';
	EXEC sp_executesql @SQL


	--SI PASO TODO OK, AHORA ACTUALIZAMOS EL GOALID EN DEVICE
	UPDATE [dbo].[Device]
		SET GoalId = @GoalId
		WHERE Id = @Id;



END