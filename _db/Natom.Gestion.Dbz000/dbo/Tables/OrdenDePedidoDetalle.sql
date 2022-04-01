CREATE TABLE [dbo].[OrdenDePedidoDetalle] (
    [OrdenDePedidoDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [OrdenDePedidoId]        INT             NOT NULL,
    [MovimientoStockId]      INT             NULL,
    [ProductoId]             INT             NOT NULL,
    [Cantidad]               INT             NOT NULL,
    [DepositoId]             INT             NOT NULL,
    [PesoUnitarioEnGramos]   INT             NOT NULL,
    [ListaDePreciosId]       INT             NULL,
    [Precio]                 DECIMAL (18, 2) NULL,
    [CantidadEntregada]      INT             NULL,
    PRIMARY KEY CLUSTERED ([OrdenDePedidoDetalleId] ASC),
    FOREIGN KEY ([DepositoId]) REFERENCES [dbo].[Deposito] ([DepositoId]),
    FOREIGN KEY ([ListaDePreciosId]) REFERENCES [dbo].[ListaDePrecios] ([ListaDePreciosId]),
    FOREIGN KEY ([MovimientoStockId]) REFERENCES [dbo].[MovimientoStock] ([MovimientoStockId]),
    FOREIGN KEY ([OrdenDePedidoId]) REFERENCES [dbo].[OrdenDePedido] ([OrdenDePedidoId]),
    FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);

