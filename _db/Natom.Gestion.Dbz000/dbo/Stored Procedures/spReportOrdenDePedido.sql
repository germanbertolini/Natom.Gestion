
CREATE PROCEDURE [dbo].[spReportOrdenDePedido]
	@OrdenDePedido INT
AS
BEGIN

	SELECT
		REPLACE(STR(OP.NumeroPedido, 8), SPACE(1), '0') AS Numero,
		OP.FechaHoraPedido AS FechaHora,
		CASE WHEN C.EsEmpresa = 1 THEN C.RazonSocial ELSE CONCAT(C.Nombre, ' ', C.Apellido) END AS Cliente,
		CASE WHEN C.EsEmpresa = 1 THEN CONCAT('CUIT ', C.NumeroDocumento) ELSE CONCAT('DNI ', C.NumeroDocumento) END AS ClienteDocumento,
		C.Domicilio AS ClienteDomicilio,
		C.Localidad AS ClienteLocalidad,
		CASE WHEN OP.NumeroRemito IS NULL OR OP.NumeroRemito = '' THEN '- SIN REMITO -' ELSE OP.NumeroRemito END AS RemitoNumero,
		CASE WHEN OP.VentaId IS NULL THEN '- Pendiente -' ELSE REPLACE(STR(V.NumeroVenta, 8), SPACE(1), '0') END AS VentaNumero,
		CASE WHEN OP.VentaId IS NULL THEN '- Pendiente -' ELSE CONCAT(V.TipoFactura, ' ', V.NumeroFactura) END AS VentaComprobante,
		COALESCE(convert(varchar, OP.EntregaEstimadaFecha, 3), '- Sin especificar -') AS EntregaFecha,
		COALESCE(RH.Descripcion, '- Sin especificar -') AS EntregaRangoHorario,
		CASE WHEN OP.RetiraPersonalmente = 1 THEN '- Retira personalmente -' ELSE OP.EntregaDomicilio END AS EntregaDomicilio,
		CASE WHEN OP.RetiraPersonalmente = 1 THEN '' ELSE OP.EntregaEntreCalles END AS EntregaEntreCalles,
		CASE WHEN OP.RetiraPersonalmente = 1 THEN '' ELSE OP.EntregaLocalidad END AS EntregaLocalidad,
		OP.EntregaTelefono1 AS EntregaTelefono1,
		OP.EntregaTelefono2 AS EntregaTelefono2,
		OP.EntregaObservaciones,
		OP.UsuarioId AS CargadoPorUsuarioId,
		OP.Observaciones,
		CASE WHEN OP.Activo = 0 THEN 'ANULADO' ELSE '' END AS Anulado,
		CASE WHEN V.VentaId IS NOT NULL THEN 'FACTURADO' ELSE '' END AS Facturado,
		CASE WHEN OP.MarcoEntregaFechaHora IS NOT NULL THEN 'ENTREGADO' ELSE '' END AS Entregado,
		OP.MontoTotal,
		(CAST(OP.PesoTotalEnGramos AS decimal(18,2)) / 1000) AS PesoTotalEnKilogramos,
		P.Codigo AS DetalleCodigo,
		P.DescripcionCorta AS DetalleDescripcion,
		D.Cantidad AS DetalleCantidad,
		CAST(D.PesoUnitarioEnGramos AS decimal(18,2)) / 1000 AS DetallePesoUnitarioEnKilogramos,
		D.Precio AS DetallePrecioUnitario,
		D.Precio * D.Cantidad AS DetallePrecioTotal,
		(CAST(D.PesoUnitarioEnGramos AS decimal(18,2)) * D.Cantidad) / 1000 AS DetallePesoTotalEnKilogramos,
		DEPO.Descripcion AS DetalleDeposito,
		L.Descripcion AS DetalleListaDePrecios,
		CASE WHEN V.VentaId IS NOT NULL THEN 'PAGADO' ELSE '' END AS Pagado
	FROM
		OrdenDePedido OP WITH(NOLOCK)
		LEFT JOIN RangoHorario RH WITH(NOLOCK) ON RH.RangoHorarioId = OP.EntregaEstimadaRangoHorarioId
		INNER JOIN Cliente C WITH(NOLOCK) ON C.ClienteId = OP.ClienteId
		LEFT JOIN Venta V WITH(NOLOCK) ON V.VentaId = OP.VentaId AND V.Activo = 1
		INNER JOIN OrdenDePedidoDetalle D WITH(NOLOCK) ON D.OrdenDePedidoId = OP.OrdenDePedidoId
		INNER JOIN Producto P WITH(NOLOCK) ON P.ProductoId = D.ProductoId
		INNER JOIN Deposito DEPO WITH(NOLOCK) ON DEPO.DepositoId = D.DepositoId
		LEFT JOIN ListaDePrecios L WITH(NOLOCK) ON L.ListaDePreciosId = D.ListaDePreciosId
	WHERE
		OP.OrdenDePedidoId = @OrdenDePedido

END