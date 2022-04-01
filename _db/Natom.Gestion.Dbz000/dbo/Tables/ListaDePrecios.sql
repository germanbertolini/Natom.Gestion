CREATE TABLE [dbo].[ListaDePrecios] (
    [ListaDePreciosId]             INT             IDENTITY (1, 1) NOT NULL,
    [Descripcion]                  NVARCHAR (50)   NULL,
    [Activo]                       BIT             NOT NULL,
    [EsPorcentual]                 BIT             DEFAULT ((0)) NOT NULL,
    [IncrementoPorcentaje]         DECIMAL (18, 2) NULL,
    [IncrementoDeListaDePreciosId] INT             NULL,
    PRIMARY KEY CLUSTERED ([ListaDePreciosId] ASC)
);

