CREATE TABLE [dbo].[RangoHorario] (
    [RangoHorarioId] INT           IDENTITY (1, 1) NOT NULL,
    [Descripcion]    NVARCHAR (50) NOT NULL,
    [Activo]         BIT           NULL,
    PRIMARY KEY CLUSTERED ([RangoHorarioId] ASC)
);

