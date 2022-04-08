CREATE TABLE [dbo].[Producto] (
    [ProductoId]          INT             IDENTITY (1, 1) NOT NULL,
    [MarcaId]             INT             NOT NULL,
    [Codigo]              NVARCHAR (15)   NULL,
    [DescripcionCorta]    NVARCHAR (50)   NULL,
    [DescripcionLarga]    NVARCHAR (200)  NULL,
    [UnidadPesoId]        INT             NOT NULL,
    [PesoUnitario]        DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [MueveStock]          BIT             DEFAULT ((1)) NOT NULL,
    [Activo]              BIT             DEFAULT ((1)) NOT NULL,
    [ProveedorId]         INT             NULL,
    [CostoUnitario]       DECIMAL (18, 2) DEFAULT ((0)) NULL,
    [CategoriaProductoId] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductoId] ASC),
    FOREIGN KEY ([MarcaId]) REFERENCES [dbo].[Marca] ([MarcaId]),
    FOREIGN KEY ([UnidadPesoId]) REFERENCES [dbo].[UnidadPeso] ([UnidadPesoId])
);

