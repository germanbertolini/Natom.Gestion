
CREATE PROCEDURE [dbo].[spPreciosList]
(
	@ListaDePreciosId INT
)
AS
BEGIN

	DECLARE @Cantidad INT = (SELECT COUNT(*) FROM vwPreciosVigentes WHERE ListaDePreciosId = COALESCE(@ListaDePreciosId, ListaDePreciosId));

	SELECT
		PV.ProductoPrecioId,
		P.ProductoId,
		CASE WHEN P.Codigo IS NULL THEN
			M.Descripcion + ' ' + P.DescripcionCorta
		ELSE
			'(' + P.Codigo + ') ' + M.Descripcion + ' ' + P.DescripcionCorta
		END AS ProductoDescripcion,
		L.ListaDePreciosId,
		L.Descripcion AS ListaDePrecioDescripcion,
		L.EsPorcentual AS ListaDePreciosEsPorcentual,
		PV.AplicaDesdeFechaHora,
		PV.Precio,
		@Cantidad AS CantidadRegistros
	FROM
		vwPreciosVigentes PV
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = PV.ProductoId
		INNER JOIN Marca M WITH(NOLOCK) ON M.MarcaId = P.MarcaId
		INNER JOIN ListaDePrecios L WITH(NOLOCK) ON L.ListaDePreciosId = PV.ListaDePreciosId
	WHERE
		PV.ListaDePreciosId = COALESCE(@ListaDePreciosId, PV.ListaDePreciosId);

END