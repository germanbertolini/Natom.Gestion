CREATE TABLE [dbo].[HistoricoCambios] (
    [HistoricoCambiosId] INT          IDENTITY (1, 1) NOT NULL,
    [UsuarioId]          INT          NULL,
    [FechaHora]          DATETIME     NOT NULL,
    [EntityName]         VARCHAR (50) NULL,
    [EntityId]           INT          NULL,
    [Accion]             VARCHAR (80) NOT NULL,
    PRIMARY KEY CLUSTERED ([HistoricoCambiosId] ASC)
);

