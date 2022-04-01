CREATE TABLE [dbo].[TipoResponsable] (
    [TipoResponsableId] INT           IDENTITY (1, 1) NOT NULL,
    [CodigoAFIP]        NVARCHAR (10) NOT NULL,
    [Descripcion]       NVARCHAR (50) NOT NULL,
    [Activo]            BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([TipoResponsableId] ASC)
);

