
CREATE PROCEDURE [dbo].[spMovimientosStockList]
(
	@DepositoId INT = NULL,
	@ProductoId INT = NULL,
	@Search NVARCHAR(100) = NULL,
	@Skip INT,
	@Take INT,
	@Fecha DATE = NULL
)
AS
BEGIN

	--GENERAMOS LA GRILLA DE MOVIMIENTOS EN UNA TABLA TEMPORAL
	SELECT
		M.MovimientoStockId,
		M.FechaHora,
		M.FechaHoraControlado,
		D.Descripcion AS Deposito,
		P.ProductoId,
		CASE WHEN P.Codigo IS NULL THEN 
			MA.Descripcion + ' ' + P.DescripcionCorta
		ELSE
			'(' + P.Codigo + ') ' + MA.Descripcion + ' ' + P.DescripcionCorta
		END AS Producto,
		M.Tipo,
		CASE WHEN M.Tipo = 'I' AND M.ConfirmacionFechaHora IS NOT NULL THEN
			M.Cantidad
		WHEN M.Tipo = 'E' AND M.ConfirmacionFechaHora IS NOT NULL THEN
			M.Cantidad * -1
		ELSE
			NULL
		END AS Movido,
		CASE WHEN M.Tipo = 'I' AND M.ConfirmacionFechaHora IS NULL THEN
			M.Cantidad
		WHEN M.Tipo = 'E' AND M.ConfirmacionFechaHora IS NULL THEN
			M.Cantidad * -1
		ELSE
			NULL
		END AS Reservado,
		M.Observaciones
	INTO
		#MOVIMIENTOS_STOCK
	FROM
		MovimientoStock M WITH(NOLOCK)
		INNER JOIN Deposito D WITH(NOLOCK) ON D.DepositoId = M.DepositoId
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = M.ProductoId
		INNER JOIN Marca MA WITH(NOLOCK) ON MA.MarcaId = P.MarcaId
	WHERE
		M.DepositoId = COALESCE(@DepositoId, M.DepositoId)
		AND M.ProductoId = COALESCE(@ProductoId, M.ProductoId)
		AND CAST(M.FechaHora AS DATE) = COALESCE(@Fecha, CAST(M.FechaHora AS DATE))
	ORDER BY
		M.MovimientoStockId DESC
		

	--TOMAMOS LA CANTIDAD DE REGISTROS EN LA GRILLA
	DECLARE @CantidadRegistros INT = COALESCE((SELECT COUNT(*) FROM #MOVIMIENTOS_STOCK), 0);


	--AHORA SELECCIONAMOS EL RANGO DE REGISTROS A MOSTRAR APLICANDO FILTROS
	SELECT
		M.MovimientoStockId,
		M.FechaHora,
		M.FechaHoraControlado,
		M.Deposito,
		M.Producto,
		M.Tipo,
		M.Movido,
		M.Reservado,
		M.Observaciones,
		[dbo].[fnCalcularStockAlMovimiento](M.MovimientoStockId, @DepositoId, M.ProductoId) AS Stock,
		@CantidadRegistros AS CantidadRegistros
	INTO
		#MOVIMIENTOS_STOCK_FILTRADOS
	FROM
		#MOVIMIENTOS_STOCK M
	WHERE
		1 = 1
		AND
		(
			@Search IS NULL
			OR
			(
				@Search IS NOT NULL
				AND
				(
					M.Deposito LIKE '%' + @Search + '%'
					OR M.Producto LIKE '%' + @Search + '%'
					OR M.Observaciones LIKE '%' + @Search + '%'
				)
			)
		);


	--TOMAMOS LA CANTIDAD DE REGISTROS FILTRADOS EN LA GRILLA
	DECLARE @CantidadFiltrados INT = COALESCE((SELECT COUNT(*) FROM #MOVIMIENTOS_STOCK_FILTRADOS), 0);

	SELECT
		*,
		@CantidadFiltrados AS CantidadFiltrados
	FROM
		#MOVIMIENTOS_STOCK_FILTRADOS M
	ORDER BY
		M.MovimientoStockId DESC
	OFFSET @Skip ROWS
	FETCH NEXT @Take ROWS ONLY;

END