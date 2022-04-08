--dbo.RangoHorario----------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [dbo].[RangoHorario] on;
;with cte_data([RangoHorarioId],[Descripcion],[Activo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Por la mañana',1),
(2,'Por la tarde',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([RangoHorarioId],[Descripcion],[Activo]))
merge	[dbo].[RangoHorario] as t
using	cte_data as s
on		1=1 and t.[RangoHorarioId] = s.[RangoHorarioId]
when matched then
	update set
	[Descripcion] = s.[Descripcion],[Activo] = s.[Activo]
when not matched by target then
	insert([RangoHorarioId],[Descripcion],[Activo])
	values(s.[RangoHorarioId],s.[Descripcion],s.[Activo])
when not matched by source then
	delete;
set identity_insert [dbo].[RangoHorario] off;


--dbo.ListaDePrecios--------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [dbo].[ListaDePrecios] on;
;with cte_data([ListaDePreciosId],[Descripcion],[Activo],[EsPorcentual],[IncrementoPorcentaje],[IncrementoDeListaDePreciosId])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Lista de precios 1',1,0,null,null)
--(2,'Lista de precios 2',1,1,-15.00,1),
--(3,'Lista de precios 3',1,1,-20.00,1),
--(4,'Lista de precios 4',1,1,-21.60,1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ListaDePreciosId],[Descripcion],[Activo],[EsPorcentual],[IncrementoPorcentaje],[IncrementoDeListaDePreciosId]))
merge	[dbo].[ListaDePrecios] as t
using	cte_data as s
on		1=1 and t.[ListaDePreciosId] = s.[ListaDePreciosId]
when matched then
	update set
	[Descripcion] = s.[Descripcion],[Activo] = s.[Activo],[EsPorcentual] = s.[EsPorcentual],[IncrementoPorcentaje] = s.[IncrementoPorcentaje],[IncrementoDeListaDePreciosId] = s.[IncrementoDeListaDePreciosId]
when not matched by target then
	insert([ListaDePreciosId],[Descripcion],[Activo],[EsPorcentual],[IncrementoPorcentaje],[IncrementoDeListaDePreciosId])
	values(s.[ListaDePreciosId],s.[Descripcion],s.[Activo],s.[EsPorcentual],s.[IncrementoPorcentaje],s.[IncrementoDeListaDePreciosId])
when not matched by source then
	delete;
set identity_insert [dbo].[ListaDePrecios] off;


--dbo.Deposito--------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [dbo].[Deposito] on;
;with cte_data([DepositoId],[Descripcion],[Activo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Galpón 1',1),
(2,'Galpón 2',1),
(3,'Local',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([DepositoId],[Descripcion],[Activo]))
merge	[dbo].[Deposito] as t
using	cte_data as s
on		1=1 and t.[DepositoId] = s.[DepositoId]
when matched then
	update set
	[Descripcion] = s.[Descripcion],[Activo] = s.[Activo]
when not matched by target then
	insert([DepositoId],[Descripcion],[Activo])
	values(s.[DepositoId],s.[Descripcion],s.[Activo])
when not matched by source then
	delete;
set identity_insert [dbo].[Deposito] off;


--dbo.TipoDocumento---------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([TipoDocumentoId],[Descripcion])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'DNI'),
(2,'CUIT')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([TipoDocumentoId],[Descripcion]))
merge	[dbo].[TipoDocumento] as t
using	cte_data as s
on		1=1 and t.[TipoDocumentoId] = s.[TipoDocumentoId]
when matched then
	update set
	[Descripcion] = s.[Descripcion]
when not matched by target then
	insert([TipoDocumentoId],[Descripcion])
	values(s.[TipoDocumentoId],s.[Descripcion])
when not matched by source then
	delete;


--dbo.TipoResponsable-------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [dbo].[TipoResponsable] on;
;with cte_data([TipoResponsableId],[CodigoAFIP],[Descripcion],[Activo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'1','IVA Responsable Inscripto',1),
(2,'2','IVA Responsable no Inscripto',1),
(3,'3','IVA no Responsable',1),
(4,'4','IVA Sujeto Exento',1),
(5,'5','Consumidor Final',1),
(6,'6','Responsable Monotributo',1),
(7,'7','Sujeto no Categorizado',0),
(8,'8','Proveedor del Exterior',0),
(9,'9','Cliente del Exterior',0),
(10,'10','IVA Liberado – Ley Nº 19.640',0),
(11,'11','IVA Responsable Inscripto – Agente de Percepción',0),
(12,'12','Pequeño Contribuyente Eventual',0),
(13,'13','Monotributista Social',0),
(14,'14','Pequeño Contribuyente Eventual Social',0)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([TipoResponsableId],[CodigoAFIP],[Descripcion],[Activo]))
merge	[dbo].[TipoResponsable] as t
using	cte_data as s
on		1=1 and t.[TipoResponsableId] = s.[TipoResponsableId]
when matched then
	update set
	[CodigoAFIP] = s.[CodigoAFIP],[Descripcion] = s.[Descripcion],[Activo] = s.[Activo]
when not matched by target then
	insert([TipoResponsableId],[CodigoAFIP],[Descripcion],[Activo])
	values(s.[TipoResponsableId],s.[CodigoAFIP],s.[Descripcion],s.[Activo])
when not matched by source then
	delete;
set identity_insert [dbo].[TipoResponsable] off;


--dbo.UnidadPeso------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [dbo].[UnidadPeso] on;
;with cte_data([UnidadPesoId],[Descripcion],[Gramos],[Mililitros])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'No aplica',NULL,NULL),
(2,'Gr',1,NULL),
(3,'Kg',1000,NULL),
(4,'Ml',NULL,1),
(5,'L',NULL,1000)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([UnidadPesoId],[Descripcion],[Gramos]))
merge	[dbo].[UnidadPeso] as t
using	cte_data as s
on		1=1 and t.[UnidadPesoId] = s.[UnidadPesoId]
when matched then
	update set
	[Descripcion] = s.[Descripcion],[Gramos] = s.[Gramos]
when not matched by target then
	insert([UnidadPesoId],[Descripcion],[Gramos])
	values(s.[UnidadPesoId],s.[Descripcion],s.[Gramos])
when not matched by source then
	delete;
set identity_insert [dbo].[UnidadPeso] off;


--dbo.Zona------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
set identity_insert [dbo].[Zona] on;
;with cte_data([ZonaId],[Descripcion],[Activo])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
(1,'Zona Norte',1),
(2,'Zona Oeste',1),
(3,'Zona Este',1),
(4,'CABA',1)
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ZonaId],[Descripcion],[Activo]))
merge	[dbo].[Zona] as t
using	cte_data as s
on		1=1 and t.[ZonaId] = s.[ZonaId]
when matched then
	update set
	[Descripcion] = s.[Descripcion],[Activo] = s.[Activo]
when not matched by target then
	insert([ZonaId],[Descripcion],[Activo])
	values(s.[ZonaId],s.[Descripcion],s.[Activo])
when not matched by source then
	delete;
set identity_insert [dbo].[Zona] off;