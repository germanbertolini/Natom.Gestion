CREATE TABLE [dbo].[Transporte] (
    [TransporteId] INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion]  NVARCHAR (50) NULL,
    [Activo]       BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([TransporteId] ASC)
);

