CREATE TABLE [dbo].[Zona](
	[ZonaId] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](50) NULL,
	[Activo] [bit] NOT NULL,
	PRIMARY KEY ([ZonaId])
)

