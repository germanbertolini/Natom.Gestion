
CREATE PROCEDURE [dbo].[spPreciosListaReport]
(
	@ListaDePreciosId INT
)
AS
BEGIN

	SELECT
		L.Descripcion AS ListaDePrecios,
		COALESCE(C.Descripcion, 'Sin categoría') AS Categoria,
		CASE WHEN P.Codigo IS NULL THEN
			P.DescripcionCorta
		ELSE
			'(' + P.Codigo + ') ' + P.DescripcionCorta
		END AS Producto,
		PV.Precio
	FROM
		[dbo].[vwPreciosVigentes] PV
		INNER JOIN ListaDePrecios L ON L.ListaDePreciosId = PV.ListaDePreciosId
		INNER JOIN Producto P ON P.ProductoId = PV.ProductoId
		LEFT JOIN CategoriaProducto C ON C.CategoriaProductoId = P.CategoriaProductoId
	WHERE
		P.Activo = 1
		AND PV.ListaDePreciosId = @ListaDePreciosId
	ORDER BY
		C.CategoriaProductoId ASC, /*P.Codigo ASC,*/ P.DescripcionCorta ASC

END