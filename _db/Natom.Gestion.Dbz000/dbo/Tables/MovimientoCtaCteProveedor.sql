CREATE TABLE [dbo].[MovimientoCtaCteProveedor] (
    [MovimientoCtaCteProveedorId] INT             IDENTITY (1, 1) NOT NULL,
    [FechaHora]                   DATETIME        NOT NULL,
    [ProveedorId]                 INT             NOT NULL,
    [UsuarioId]                   INT             NULL,
    [Tipo]                        CHAR (1)        NOT NULL,
    [Importe]                     DECIMAL (18, 2) NOT NULL,
    [Observaciones]               NVARCHAR (200)  NULL,
    [CompraMovimientoStockId]     INT             NULL,
    PRIMARY KEY CLUSTERED ([MovimientoCtaCteProveedorId] ASC),
    FOREIGN KEY ([CompraMovimientoStockId]) REFERENCES [dbo].[MovimientoStock] ([MovimientoStockId]),
    FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId])
);

