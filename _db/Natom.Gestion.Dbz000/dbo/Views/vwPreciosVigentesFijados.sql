
CREATE VIEW [dbo].[vwPreciosVigentesFijados]
AS
	SELECT
		ProductoPrecioId,
		ProductoId,
		ListaDePreciosId,
		Precio,
		AplicaDesdeFechaHora,
		HistoricoReajustePrecioId
	FROM
		ProductoPrecio WITH(NOLOCK)
	WHERE
		ProductoPrecioId IN (SELECT MAX(ProductoPrecioId) FROM ProductoPrecio WITH(NOLOCK) WHERE FechaHoraBaja IS NULL GROUP BY ProductoId, ListaDePreciosId);