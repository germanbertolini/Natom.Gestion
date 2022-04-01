CREATE PROCEDURE [dbo].[sp_usuarios_select_by_id]
(
	@UsuarioId INT
)
AS
BEGIN

	--Devuelve Resultset con los datos del usuario
	SELECT
		*
	FROM
		Usuario WITH(NOLOCK)
	WHERE
		UsuarioId = @UsuarioId;


	--Devuelve segundo Resultset con los permisos del usuario (si es que sigue activo)
	SELECT
		*
	FROM
		[dbo].[UsuarioPermiso] WITH(NOLOCK)
	WHERE
		[UsuarioId] = @UsuarioId;
	

END