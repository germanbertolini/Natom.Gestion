CREATE TABLE [dbo].[Device]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[InstanceId] CHAR(32) NOT NULL,
	[DeviceId] INT NOT NULL,
	[DeviceName] NVARCHAR(50) NOT NULL,
	[DeviceIP] NVARCHAR(50),
	[DeviceUser] NVARCHAR(50),
	[DevicePassword] NVARCHAR(50),
	[GoalId] INT,
	[AddedAt] DATETIME NOT NULL,
	[LastConfigurationAt] DATETIME,
	[LastSyncAt] DATETIME,
	[SerialNumber] NVARCHAR(50),
	[Model] NVARCHAR(30),
	[Brand] NVARCHAR(30),
	[DateTimeFormat] NVARCHAR(20),
	[FirmwareVersion] NVARCHAR(30)
);