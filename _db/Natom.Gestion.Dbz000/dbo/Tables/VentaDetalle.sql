CREATE TABLE [dbo].[VentaDetalle] (
    [VentaDetalleId]         INT             IDENTITY (1, 1) NOT NULL,
    [VentaId]                INT             NOT NULL,
    [ProductoId]             INT             NOT NULL,
    [Cantidad]               INT             NOT NULL,
    [DepositoId]             INT             NOT NULL,
    [OrdenDePedidoId]        INT             NULL,
    [OrdenDePedidoDetalleId] INT             NULL,
    [NumeroRemito]           NVARCHAR (20)   NULL,
    [PesoUnitarioEnGramos]   INT             NULL,
    [ListaDePreciosId]       INT             NULL,
    [Precio]                 DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([VentaDetalleId] ASC),
    FOREIGN KEY ([DepositoId]) REFERENCES [dbo].[Deposito] ([DepositoId]),
    FOREIGN KEY ([ListaDePreciosId]) REFERENCES [dbo].[ListaDePrecios] ([ListaDePreciosId]),
    FOREIGN KEY ([OrdenDePedidoDetalleId]) REFERENCES [dbo].[OrdenDePedidoDetalle] ([OrdenDePedidoDetalleId]),
    FOREIGN KEY ([OrdenDePedidoId]) REFERENCES [dbo].[OrdenDePedido] ([OrdenDePedidoId]),
    FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId]),
    FOREIGN KEY ([VentaId]) REFERENCES [dbo].[Venta] ([VentaId])
);

