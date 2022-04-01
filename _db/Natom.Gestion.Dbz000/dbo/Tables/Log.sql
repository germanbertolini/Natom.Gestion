CREATE TABLE [dbo].[Log] (
    [LogId]     INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioId] INT            NULL,
    [FechaHora] DATETIME       NOT NULL,
    [UserAgent] NVARCHAR (400) NULL,
    [Exception] NVARCHAR (MAX) NULL
);

