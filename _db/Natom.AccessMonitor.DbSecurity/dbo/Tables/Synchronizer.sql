CREATE TABLE [dbo].[Synchronizer]
(
	[InstanceId] CHAR(32) NOT NULL,
	[InstallationAlias] NVARCHAR(50) NOT NULL,
	[InstallerName] NVARCHAR(50) NOT NULL,
	[InstalledAt] DATETIME NOT NULL,
	[ActivatedAt] DATETIME,
	[LastSyncAt] DATETIME,
	[LastConnectionAt] DATETIME,
	[CurrentSyncToServerMinutes] INT,
	[NewSyncToServerMinutes] INT,
	[CurrentSyncFromDevicesMinutes] INT,
	[NewSyncFromDevicesMinutes] INT,
	[ClientId] INT,
	[RemovedAt] DATETIME,
	CONSTRAINT [PK_Synchronizer] PRIMARY KEY CLUSTERED ([InstanceId] ASC)
);
