CREATE TABLE NegocioConfig
(
	NegocioConfigId INT NOT NULL PRIMARY KEY,
	FechaMembresia DATE,
	RazonSocial NVARCHAR(50),
	NombreFantasia NVARCHAR(50),
	TipoDocumento NVARCHAR(20),
	NumeroDocumento NVARCHAR(20),
	Domicilio NVARCHAR(50),
	Localidad NVARCHAR(50),
	Telefono NVARCHAR(30),
	Email NVARCHAR(50),
	LogoBase64 NVARCHAR(MAX)
);