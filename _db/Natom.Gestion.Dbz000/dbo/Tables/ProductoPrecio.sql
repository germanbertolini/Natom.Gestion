CREATE TABLE [dbo].[ProductoPrecio] (
    [ProductoPrecioId]          INT             IDENTITY (1, 1) NOT NULL,
    [ProductoId]                INT             NOT NULL,
    [ListaDePreciosId]          INT             NULL,
    [Precio]                    DECIMAL (18, 2) NOT NULL,
    [AplicaDesdeFechaHora]      DATETIME        NOT NULL,
    [FechaHoraBaja]             DATETIME        NULL,
    [HistoricoReajustePrecioId] INT             NULL,
    PRIMARY KEY CLUSTERED ([ProductoPrecioId] ASC),
    FOREIGN KEY ([ListaDePreciosId]) REFERENCES [dbo].[ListaDePrecios] ([ListaDePreciosId]),
    FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);

