CREATE TABLE [dbo].[ClienteMonto]
(
	ClienteMontoId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ClienteId INT NOT NULL,
	Monto DECIMAL(18,2) NOT NULL,
	Desde DATETIME NOT NULL,
	UsuarioId INT NOT NULL,
	FechaHoraAnulado DATETIME
);
