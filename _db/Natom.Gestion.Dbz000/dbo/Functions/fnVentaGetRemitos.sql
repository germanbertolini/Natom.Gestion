
CREATE FUNCTION fnVentaGetRemitos
(
	@VentaId INT
) RETURNS NVARCHAR(MAX)
AS
BEGIN

	DECLARE @Return NVARCHAR(MAX) = '';

	SELECT
		@Return = STRING_AGG(CONCAT('RTO ', R.NumeroRemito), ', ')
	FROM
	(
		SELECT DISTINCT
			V.VentaId,
			OP.NumeroRemito
		FROM
			Venta V WITH(NOLOCK)
			INNER JOIN VentaDetalle D WITH(NOLOCK) ON D.VentaId = V.VentaId
			INNER JOIN OrdenDePedidoDetalle OPD WITH(NOLOCK) ON OPD.OrdenDePedidoDetalleId = D.OrdenDePedidoDetalleId
			INNER JOIN OrdenDePedido OP WITH(NOLOCK) ON OP.OrdenDePedidoId = OPD.OrdenDePedidoId AND OP.NumeroRemito IS NOT NULL AND OP.NumeroRemito != '' AND OP.Activo = 1
		WHERE
			V.VentaId = @VentaId
	) AS R
	GROUP BY
		R.VentaId;

	RETURN @Return;

END