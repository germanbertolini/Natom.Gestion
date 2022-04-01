CREATE TABLE [dbo].[HistoricoCambiosMotivo] (
    [HistoricoCambiosMotivoId] INT            IDENTITY (1, 1) NOT NULL,
    [HistoricoCambiosId]       INT            NOT NULL,
    [Motivo]                   NVARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([HistoricoCambiosMotivoId] ASC),
    FOREIGN KEY ([HistoricoCambiosId]) REFERENCES [dbo].[HistoricoCambios] ([HistoricoCambiosId])
);

