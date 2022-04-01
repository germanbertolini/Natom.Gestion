CREATE TABLE [dbo].[Permiso]
(
	[PermisoId] NVARCHAR(50) NOT NULL,
	[Scope] NVARCHAR(20) NOT NULL,
	[Descripcion] NVARCHAR(200),
	PRIMARY KEY (PermisoId, Scope)
);