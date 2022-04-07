CREATE TABLE [dbo].[CategoriaProducto] (
    [CategoriaProductoId] INT           NOT NULL IDENTITY(1, 1),
    [Descripcion]         NVARCHAR (50) NOT NULL,
    [Eliminado]           BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CategoriaProducto] PRIMARY KEY CLUSTERED ([CategoriaProductoId] ASC)
);

