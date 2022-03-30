CREATE TABLE [dbo].[DocketRange] (
	[DocketRangeId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	DocketId INT NOT NULL,
	[DayOfWeek] INT NOT NULL,
	[From] TIME,
	[To] TIME
)