CREATE TABLE [dbo].[HistoricoReajustePrecio] (
    [HistoricoReajustePrecioId] INT             IDENTITY (1, 1) NOT NULL,
    [FechaHora]                 DATETIME        NOT NULL,
    [UsuarioId]                 INT             NOT NULL,
    [EsIncremento]              BIT             NOT NULL,
    [EsPorcentual]              BIT             NOT NULL,
    [Valor]                     DECIMAL (18, 2) NOT NULL,
    [AplicoMarcaId]             INT             NOT NULL,
    [AplicoListaDePreciosId]    INT             NULL,
    [AplicaDesdeFechaHora]      DATETIME        NOT NULL,
    [FechaHoraBaja]             DATETIME        NULL,
    PRIMARY KEY CLUSTERED ([HistoricoReajustePrecioId] ASC),
    FOREIGN KEY ([AplicoMarcaId]) REFERENCES [dbo].[Marca] ([MarcaId])
);

