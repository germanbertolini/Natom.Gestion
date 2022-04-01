
CREATE PROCEDURE [dbo].[spPrecioGet]
(
	@ListaDePreciosId INT = NULL,
	@ProductoId INT
)
AS
BEGIN

	SELECT top 100 --parche 21-03-22 para poder listar
		ProductoPrecioId,
		Precio,
		ListaDePreciosId
	FROM
		vwPreciosVigentes PV
	WHERE
		PV.ListaDePreciosId = COALESCE(@ListaDePreciosId, PV.ListaDePreciosId)
		AND PV.ProductoId = @ProductoId;

END