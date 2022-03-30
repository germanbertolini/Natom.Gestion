CREATE PROCEDURE [dbo].[sp_movements_preinsert]
(
	@ClientId INT
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']'') AND type in (N''U''))
			BEGIN
				CREATE TABLE [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']
				(
					[MovementId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
					[InstanceId] CHAR(32) NOT NULL,
					[DeviceId] INT NOT NULL,
					[DateTime] DATETIME NOT NULL,
					[DocketNumber] INT NOT NULL,
					[MovementType] CHAR(1) NOT NULL,
					[GoalId] INT,
					[ProcessedAt] DATETIME
				);

				CREATE UNIQUE INDEX [IDX_zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '] ON [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']([DateTime], [InstanceId], [DeviceId], [DocketNumber]);
			END

		IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed]'') AND type in (N''U''))
			CREATE TABLE [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed]
			(
				[Date] DATE NOT NULL,
				[DocketNumber] NVARCHAR(20) NOT NULL,
				[DocketId] INT,
				[ExpectedPlaceId] INT NOT NULL,
				[ExpectedIn] TIME,
				[In] DATETIME,
				[InGoalId] INT,
				[InPlaceId] INT,
				[InDeviceId] INT,
				[InDeviceMovementType] CHAR(1),
				[InWasEstimated] BIT,
				[InProcessedAt] DATETIME,

				[ExpectedOut] TIME,
				[Out] DATETIME,
				[OutGoalId] INT,
				[OutPlaceId] INT,
				[OutDeviceId] INT,
				[OutDeviceMovementType] CHAR(1),
				[OutWasEstimated] BIT,
				[OutProcessedAt] DATETIME,

				[PermanenceTime] TIME,

				CONSTRAINT [PK_zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] PRIMARY KEY CLUSTERED ([Date] ASC, [DocketId] ASC, [ExpectedIn] ASC)
			);
	';

	EXEC sp_executesql @SQL

END