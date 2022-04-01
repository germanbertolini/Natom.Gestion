CREATE PROCEDURE [dbo].[sp_permisos_select_by_scope]
(
	@Scope NVARCHAR(20)
)
AS
BEGIN

	SELECT
		*
	FROM
		Permiso
	WHERE
		Scope = @Scope;
	

END