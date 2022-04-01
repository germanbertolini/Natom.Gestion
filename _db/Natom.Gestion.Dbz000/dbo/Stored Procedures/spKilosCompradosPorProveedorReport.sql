
CREATE PROCEDURE spKilosCompradosPorProveedorReport
(
	@Desde DATE = NULL
)
AS
BEGIN

	SELECT
		SUM(M.Cantidad * CAST(P.PesoUnitario AS decimal(18,2)) * UP.Gramos / 1000) AS TotalKilosComprados,
		CASE WHEN PR.EsEmpresa = 1THEN
			PR.RazonSocial
		ELSE
			PR.Nombre + ' ' + PR.Apellido
		END AS Proveedor,
		CASE WHEN PR.EsEmpresa = 1 THEN
			'CUIT ' + PR.NumeroDocumento
		ELSE
			'DNI ' + PR.NumeroDocumento
		END AS Documento
	FROM
		MovimientoStock M WITH(NOLOCK)
		INNER JOIN Proveedor PR WITH(NOLOCK) ON PR.ProveedorId = M.ProveedorId
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = M.ProductoId
		INNER JOIN UnidadPeso UP WITH(NOLOCK) ON UP.UnidadPesoId = P.UnidadPesoId
	WHERE
		M.Tipo = 'I'
		AND M.EsCompra = 1
		AND M.FechaHora >= COALESCE(@Desde, M.FechaHora)
	GROUP BY
		PR.ProveedorId, PR.RazonSocial, PR.Nombre, PR.Apellido, PR.EsEmpresa, PR.NumeroDocumento
	ORDER BY
		SUM(M.Cantidad * CAST(P.PesoUnitario AS decimal(18,2)) / 1000) DESC;


END