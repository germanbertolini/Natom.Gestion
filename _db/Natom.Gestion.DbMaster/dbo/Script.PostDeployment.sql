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