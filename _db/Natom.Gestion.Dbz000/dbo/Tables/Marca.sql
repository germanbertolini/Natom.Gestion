CREATE TABLE [dbo].[Marca] (
    [MarcaId]     INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion] NVARCHAR (50) NULL,
    [Activo]      BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([MarcaId] ASC)
);

