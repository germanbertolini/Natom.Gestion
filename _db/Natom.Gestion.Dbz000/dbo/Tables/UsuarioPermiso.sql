CREATE TABLE [dbo].[UsuarioPermiso] (
    [UsuarioPermisoId] INT           IDENTITY (1, 1) NOT NULL,
    [UsuarioId]        INT           NOT NULL,
    [PermisoId]        NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([UsuarioPermisoId] ASC),
    FOREIGN KEY ([PermisoId]) REFERENCES [dbo].[Permiso] ([PermisoId]),
    FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([UsuarioId])
);

