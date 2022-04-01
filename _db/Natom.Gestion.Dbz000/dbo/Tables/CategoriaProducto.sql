CREATE TABLE [dbo].[CategoriaProducto] (
    [CategoriaProductoId] NVARCHAR (40) NOT NULL,
    [Descripcion]         NVARCHAR (50) NOT NULL,
    [Eliminado]           BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CategoriaProducto] PRIMARY KEY CLUSTERED ([CategoriaProductoId] ASC)
);

