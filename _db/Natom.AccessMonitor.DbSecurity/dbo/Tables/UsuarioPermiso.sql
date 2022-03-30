  CREATE TABLE [dbo].[UsuarioPermiso]
(
	[UsuarioPermisoId] INT NOT NULL IDENTITY(1,1),
	[UsuarioId] INT NOT NULL,
	[PermisoId] NVARCHAR(50) NOT NULL,
	[Scope] NVARCHAR(20) NOT NULL,
	PRIMARY KEY (UsuarioPermisoId),
	FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId),
	FOREIGN KEY (PermisoId, Scope) REFERENCES Permiso(PermisoId, Scope)
);
