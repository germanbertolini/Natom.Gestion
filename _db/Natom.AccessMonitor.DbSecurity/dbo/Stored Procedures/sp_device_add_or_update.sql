CREATE PROCEDURE [dbo].[sp_device_add_or_update]
(
	@InstanceId CHAR(32),
	@DeviceId INT,
	@DeviceName NVARCHAR(50),
	@LastConfigurationAt DATETIME = NULL,
	@SerialNumber NVARCHAR(50),
	@Model NVARCHAR(30),
	@Brand NVARCHAR(30),
	@DateTimeFormat NVARCHAR(20),
	@FirmwareVersion NVARCHAR(30),
	@IP NVARCHAR(50),
	@User NVARCHAR(50),
	@Pass NVARCHAR(50),
	@LastSyncRegistered DATETIME
)
AS
BEGIN

	DECLARE @currentDeviceName NVARCHAR(50);
	DECLARE @currentLastConfigurationAt DATETIME;
	DECLARE @currentGoalId INT = NULL;

	SELECT
		@currentDeviceName = DeviceName,
		@currentLastConfigurationAt = LastConfigurationAt,
		@currentGoalId = GoalId
	FROM
		[dbo].[Device]
	WHERE
		InstanceId = @InstanceId
		AND DeviceId = @DeviceId;

	--SI NO EXISTE EL DEVICE LO INSERTAMOS
	IF @currentDeviceName IS NULL
		INSERT INTO [dbo].[Device] (InstanceId, DeviceId, DeviceName, GoalId, AddedAt,
									LastConfigurationAt, SerialNumber, Model, Brand,
									DateTimeFormat, FirmwareVersion, DeviceIP, DeviceUser, DevicePassword, LastSyncAt)
				VALUES (@InstanceId, @DeviceId, @DeviceName, NULL, GETDATE(),
						@LastConfigurationAt, @SerialNumber, @Model, @Brand,
						@DateTimeFormat, @FirmwareVersion, @IP, @User, @Pass, @LastSyncRegistered);

	--SI YA EXISTE EL DEVICE Y CAMBIO LA CONFIGURACIÓN, LO ACTUALIZAMOS
	ELSE IF @currentDeviceName IS NOT NULL AND (@currentLastConfigurationAt IS NULL OR @currentLastConfigurationAt != @LastConfigurationAt)
		UPDATE [dbo].[Device]
			SET DeviceName = @DeviceName,
				LastConfigurationAt = @LastConfigurationAt,
				SerialNumber = @SerialNumber,
				Model = @Model,
				Brand = @Brand,
				DateTimeFormat = @DateTimeFormat,
				FirmwareVersion = @FirmwareVersion,
				DeviceIP = @IP,
				DeviceUser = @User,
				DevicePassword = @Pass,
				LastSyncAt = @LastSyncRegistered
			WHERE InstanceId = @InstanceId
					AND DeviceId = @DeviceId;
	--SI ESTÁ TODO OK ENTONCES SOLAMENTE REFRESCAMOS LA ULTIMA FECHA HORA DE SINCRONIZACIÓN
	ELSE
		UPDATE [dbo].[Device]
			SET LastSyncAt = @LastSyncRegistered
			WHERE InstanceId = @InstanceId
					AND DeviceId = @DeviceId;


	--DEVOLVEMOS LA INFO DEL DEVICE
	SELECT
		@currentGoalId AS CurrentGoalId;

END