CREATE TABLE [dbo].[TransactionLog] (
  [TraceTransactionId] CHAR(32) DEFAULT NULL,
  [DateTime] DATETIME NOT NULL,
  [Type] CHAR(5) NOT NULL,
  [Description] VARCHAR(100) DEFAULT NULL,
  [Data] TEXT DEFAULT NULL,
  INDEX [IDX_TransactionLog_TraceTransactionId] ([TraceTransactionId])
);