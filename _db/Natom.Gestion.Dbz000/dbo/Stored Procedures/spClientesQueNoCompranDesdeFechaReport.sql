
CREATE PROCEDURE spClientesQueNoCompranDesdeFechaReport
(
	@Desde DATE
)
AS
BEGIN

	SELECT
		CASE WHEN C.EsEmpresa = 1 THEN
			C.RazonSocial
		ELSE
			C.Nombre + ' ' + C.Apellido
		END AS Cliente,
		CASE WHEN C.EsEmpresa = 1 THEN
			'CUIT ' + C.NumeroDocumento
		ELSE
			'DNI ' + C.NumeroDocumento
		END AS Documento,
		UC.FechaHora AS FechaHoraUltimaCompra
	FROM
		Cliente C WITH(NOLOCK)
		INNER JOIN vwClientesUltimaCompra UC ON UC.ClienteId = C.ClienteId
	WHERE
		UC.FechaHora <= @Desde
	ORDER BY
		UC.FechaHora DESC;


END