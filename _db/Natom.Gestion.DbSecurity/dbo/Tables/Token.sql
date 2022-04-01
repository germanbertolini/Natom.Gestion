CREATE TABLE [dbo].[Token]
(
	[Key] NVARCHAR(32) NOT NULL,
	[Scope] NVARCHAR(20) NOT NULL,
	[SyncInstanceId] NVARCHAR(32),
	[UserId] INT,
	[UserFullName] VARCHAR(100),
	[ClientId] INT,
	[ClientFullName] VARCHAR(100),
	[CreatedAt] DATETIME NOT NULL,
	[ExpiresAt] DATETIME,
	CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED ([Scope] ASC, [Key] ASC)
);