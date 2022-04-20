CREATE TABLE [dbo].[ClienteCobranza]
(
	ClienteCobranzaId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ClienteId INT NOT NULL,
	Monto DECIMAL(18,2) NOT NULL,
	FechaHora DATETIME NOT NULL,
	ClienteMontoId INT NOT NULL
);