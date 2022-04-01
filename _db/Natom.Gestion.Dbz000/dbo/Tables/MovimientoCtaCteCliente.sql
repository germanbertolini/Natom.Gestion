CREATE TABLE [dbo].[MovimientoCtaCteCliente] (
    [MovimientoCtaCteClienteId] INT             IDENTITY (1, 1) NOT NULL,
    [FechaHora]                 DATETIME        NOT NULL,
    [ClienteId]                 INT             NOT NULL,
    [UsuarioId]                 INT             NULL,
    [Tipo]                      CHAR (1)        NOT NULL,
    [Importe]                   DECIMAL (18, 2) NOT NULL,
    [Observaciones]             NVARCHAR (200)  NULL,
    [VentaId]                   INT             NULL,
    PRIMARY KEY CLUSTERED ([MovimientoCtaCteClienteId] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Cliente] ([ClienteId])
);

