
CREATE PROCEDURE spReportVenta
	@VentaId INT
AS
BEGIN

	SELECT
		CASE WHEN CHARINDEX('-',REVERSE(TipoFactura)) = 0 THEN 
			SUBSTRING(TipoFactura, 1, 1)
		ELSE
			SUBSTRING(TipoFactura, LEN(TipoFactura) - CHARINDEX('-',REVERSE(TipoFactura)) + 2, LEN(TipoFactura))
		END AS LetraComprobante,
		CASE WHEN TipoFactura LIKE 'FCE%' THEN
			'FACTURA ELECTRÓNICA'
		WHEN TipoFactura LIKE 'FC%' THEN
			'FACTURA'
		WHEN TipoFactura = 'PRE' THEN
			'PRESUPUESTO'
		WHEN TipoFactura = 'RBO' THEN
			'RECIBO'
		END AS Comprobante,
		REPLACE(STR(V.NumeroVenta, 8), SPACE(1), '0') AS VentaNumero,
		V.NumeroFactura AS NumeroComprobante,
		V.FechaHoraVenta AS FechaHora,
		CASE WHEN C.EsEmpresa = 1 THEN C.RazonSocial ELSE CONCAT(C.Nombre, ' ', C.Apellido) END AS Cliente,
		CASE WHEN C.EsEmpresa = 1 THEN CONCAT('CUIT ', C.NumeroDocumento) ELSE CONCAT('DNI ', C.NumeroDocumento) END AS ClienteDocumento,
		C.Domicilio AS ClienteDomicilio,
		C.Localidad AS ClienteLocalidad,
		dbo.fnVentaGetRemitos(V.VentaId) AS Remitos,
		V.UsuarioId AS FacturadoPorUsuarioId,
		V.Observaciones,
		CASE WHEN V.Activo = 0 THEN 'ANULADA' ELSE '' END AS Anulado,
		V.MontoTotal,
		CAST(V.PesoTotalEnGramos AS decimal(18,2)) / 1000 AS PesoTotalEnKilogramos,
		P.Codigo AS DetalleCodigo,
		P.DescripcionCorta AS DetalleDescripcion,
		CASE WHEN OP.NumeroRemito IS NULL THEN '' ELSE OP.NumeroRemito END AS DetalleRemito,
		D.Cantidad AS DetalleCantidad,
		CAST(D.PesoUnitarioEnGramos AS decimal(18,2)) / 1000 AS DetallePesoUnitarioEnKilogramos,
		D.Precio AS DetallePrecioUnitario,
		(CAST(D.PesoUnitarioEnGramos AS decimal(18,2)) / 1000) * D.Cantidad AS DetallePesoTotalEnKilogramos,
		(D.Precio) * D.Cantidad AS DetallePrecioTotal,
		L.Descripcion AS DetalleListaDePrecios
	FROM
		Venta V WITH(NOLOCK)
		INNER JOIN Cliente C WITH(NOLOCK) ON C.ClienteId = V.ClienteId
		INNER JOIN VentaDetalle D WITH(NOLOCK) ON D.VentaId = V.VentaId
		LEFT JOIN OrdenDePedidoDetalle OPD WITH(NOLOCK) ON OPD.OrdenDePedidoDetalleId = D.OrdenDePedidoDetalleId
		LEFT JOIN OrdenDePedido OP WITH(NOLOCK) ON OP.OrdenDePedidoId = OPD.OrdenDePedidoId AND OP.NumeroRemito IS NOT NULL AND OP.NumeroRemito != ''
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = D.ProductoId
		LEFT JOIN ListaDePrecios L WITH(NOLOCK) ON L.ListaDePreciosId = D.ListaDePreciosId
	WHERE
		V.VentaId = @VentaId

END