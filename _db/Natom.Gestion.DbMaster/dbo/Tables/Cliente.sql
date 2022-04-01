CREATE TABLE [dbo].[Cliente]
(
	ClienteId INT NOT NULL IDENTITY(1,1),
	EsEmpresa BIT NOT NULL,
	RegisterAt DATETIME,
	Nombre NVARCHAR(50),
	Apellido NVARCHAR(50),
	RazonSocial NVARCHAR(50),
	NombreFantasia NVARCHAR(50),
	TipoDocumentoId INT NOT NULL,
	NumeroDocumento NVARCHAR(20) NOT NULL,
	ZonaId INT,
	Domicilio NVARCHAR(50),
	EntreCalles NVARCHAR(50),
	Localidad NVARCHAR(50),
	ContactoTelefono1 NVARCHAR(30),
	ContactoTelefono2 NVARCHAR(30),
	ContactoEmail1 NVARCHAR(50),
	ContactoEmail2 NVARCHAR(50),
	ContactoObservaciones NVARCHAR(200),
	Activo BIT NOT NULL,

	MovementProcess_StartDateTime DATETIME,
	MovementProcess_Running BIT DEFAULT 0,
	MovementProcess_EndDateTime DATETIME,
	MovementProcess_EndWithSuccess BIT,
	MovementProcess_LastTurnosRun DATETIME

	PRIMARY KEY (ClienteId)
)
