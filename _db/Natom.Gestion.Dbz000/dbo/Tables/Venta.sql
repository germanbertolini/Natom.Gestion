CREATE TABLE [dbo].[Venta] (
    [VentaId]           INT             IDENTITY (1, 1) NOT NULL,
    [NumeroVenta]       INT             NOT NULL,
    [ClienteId]         INT             NOT NULL,
    [FechaHoraVenta]    DATETIME        NOT NULL,
    [UsuarioId]         INT             NULL,
    [TipoFactura]       NVARCHAR (10)   NULL,
    [NumeroFactura]     NVARCHAR (20)   NULL,
    [Activo]            BIT             NULL,
    [Observaciones]     NVARCHAR (200)  NULL,
    [MontoTotal]        DECIMAL (18, 2) NOT NULL,
    [PesoTotalEnGramos] DECIMAL (18, 2) NOT NULL,
    [PagoReferencia]    NVARCHAR (50)   NULL,
    [MedioDePago]       NVARCHAR (30)   NULL,
    PRIMARY KEY CLUSTERED ([VentaId] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Cliente] ([ClienteId])
);

