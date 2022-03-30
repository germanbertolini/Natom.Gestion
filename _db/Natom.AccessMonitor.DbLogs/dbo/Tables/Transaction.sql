CREATE TABLE [dbo].[Transaction] (
  [TraceTransactionId] CHAR(32) NOT NULL,
  [IP] varchar(50) DEFAULT NULL,
  [UserId] BIGINT DEFAULT NULL,
  [UrlRequested] VARCHAR(200) DEFAULT NULL,
  [ActionRequested] VARCHAR(200) DEFAULT NULL,
  [DateTime] DATETIME NOT NULL,
  [OS] VARCHAR(50) DEFAULT NULL,
  [AppVersion] VARCHAR(10) DEFAULT NULL,
  [Lang] char(2) DEFAULT NULL,
  [Scope] VARCHAR(20) NOT NULL,
  [InstanceId] VARCHAR(32) NULL,
  [HostName] VARCHAR(50) DEFAULT NULL,
  [Port] INT DEFAULT NULL
)