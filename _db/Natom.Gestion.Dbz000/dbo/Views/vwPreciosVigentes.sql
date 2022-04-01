
CREATE VIEW [dbo].[vwPreciosVigentes]
AS
	--LISTAS DE PRECIOS NO PORCENTUALES
	SELECT
		ProductoPrecioId,
		ProductoId,
		ListaDePreciosId,
		Precio,
		AplicaDesdeFechaHora,
		HistoricoReajustePrecioId
	FROM
		[dbo].[vwPreciosVigentesFijados]

	UNION

	--LISTAS DE PRECIOS PORCENTUALES
	SELECT
		NULL AS ProductoPrecioId,
		P.ProductoId,
		L.ListaDePreciosId,
		CASE WHEN L.IncrementoPorcentaje < 0 THEN
			ROUND(((100 + L.IncrementoPorcentaje) * P.Precio) / 100, 0)
		ELSE
			ROUND(P.Precio * ((L.IncrementoPorcentaje / 100) + 1), 0)
		END AS Precio,
		P.AplicaDesdeFechaHora,
		NULL AS HistoricoReajustePrecioId
	FROM
		ListaDePrecios L WITH(NOLOCK)
		INNER JOIN [dbo].[vwPreciosVigentesFijados] P ON P.ListaDePreciosId = L.IncrementoDeListaDePreciosId
		LEFT JOIN [dbo].[vwPreciosVigentesFijados] DEFINIDO ON DEFINIDO.ProductoId = P.ProductoId AND DEFINIDO.ListaDePreciosId = L.ListaDePreciosId
	WHERE
		L.Activo = 1
		AND L.EsPorcentual = 1
		AND DEFINIDO.ProductoPrecioId IS NULL --Y QUE NO ESTÉ DEFINIDO DENTRO DE LA LISTA PORCENTUAL