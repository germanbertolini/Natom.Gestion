CREATE TABLE [dbo].[Usuario]
(
	[UsuarioId] INT NOT NULL IDENTITY(1,1),
	[Scope] NVARCHAR(20) NOT NULL,
	[ClienteId] INT NOT NULL,
	[Nombre] NVARCHAR(50) NOT NULL,
	[Apellido] NVARCHAR(50) NOT NULL,
	[Email] NVARCHAR(50) NOT NULL,
	[Clave] NVARCHAR(32),
	[FechaHoraConfirmacionEmail] DATETIME,
	[FechaHoraUltimoEmailEnviado] DATETIME,
	[SecretConfirmacion] CHAR(32),
	[FechaHoraAlta] DATETIME NOT NULL,
	[FechaHoraBaja] DATETIME DEFAULT NULL,
	PRIMARY KEY (UsuarioId)
);