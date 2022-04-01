CREATE TABLE [dbo].[MovimientoCajaDiaria] (
    [MovimientoCajaDiariaId] INT             IDENTITY (1, 1) NOT NULL,
    [FechaHora]              DATETIME        NOT NULL,
    [UsuarioId]              INT             NULL,
    [Tipo]                   CHAR (1)        NOT NULL,
    [Importe]                DECIMAL (18, 2) NOT NULL,
    [Observaciones]          NVARCHAR (200)  NULL,
    [EsCheque]               BIT             DEFAULT ((0)) NOT NULL,
    [VentaId]                INT             NULL,
    [MedioDePago]            NVARCHAR (30)   NULL,
    [Referencia]             NVARCHAR (50)   NULL,
    PRIMARY KEY CLUSTERED ([MovimientoCajaDiariaId] ASC)
);

