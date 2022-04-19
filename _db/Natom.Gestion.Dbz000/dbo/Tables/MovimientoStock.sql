CREATE TABLE [dbo].[MovimientoStock] (
    [MovimientoStockId]     INT             IDENTITY (1, 1) NOT NULL,
    [ProductoId]            INT             NOT NULL,
    [FechaHora]             DATETIME        NOT NULL,
    [UsuarioId]             INT             NULL,
    [Tipo]                  CHAR (1)        NOT NULL,
    [Cantidad]              DECIMAL (18, 2) NOT NULL,
    [ConfirmacionFechaHora] DATETIME        NULL,
    [ConfirmacionUsuarioId] INT             NULL,
    [DepositoId]            INT             NOT NULL,
    [Observaciones]         NVARCHAR (200)  NULL,
    [EsCompra]              BIT             DEFAULT ((0)) NOT NULL,
    [ProveedorId]           INT             NULL,
    [CostoUnitario]         DECIMAL (18, 2) NULL,
    [FechaHoraControlado]   DATETIME        NULL,
    [ControladoUsuarioId]   INT             NULL,
    PRIMARY KEY CLUSTERED ([MovimientoStockId] ASC),
    FOREIGN KEY ([DepositoId]) REFERENCES [dbo].[Deposito] ([DepositoId]),
    FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);

