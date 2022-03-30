﻿CREATE TABLE [dbo].[Place]
(
	[PlaceId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[Name] NVARCHAR(50) NOT NULL,
	[Address] NVARCHAR(100) NOT NULL,
	[Lat] DECIMAL(12,9),
	[Lng] DECIMAL(12,9),
	[ClientId] INT NOT NULL,
	[RemovedAt] DATETIME
);