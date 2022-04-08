CREATE TABLE [dbo].[UnidadPeso] (
    [UnidadPesoId] INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion]  NVARCHAR (10) NULL,
    [Gramos]       INT           NULL,
    [Mililitros]   INT           NULL,
    PRIMARY KEY CLUSTERED ([UnidadPesoId] ASC)
);

