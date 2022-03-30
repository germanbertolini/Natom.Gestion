CREATE TABLE [dbo].[Config] (
    [Clave]       NVARCHAR (100)  NOT NULL,
    [Valor]       NVARCHAR (255) NULL,
    [Description] NVARCHAR (300) NULL,
    CONSTRAINT [PK_Config] PRIMARY KEY CLUSTERED ([Clave] ASC)
);

