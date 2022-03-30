CREATE PROCEDURE [dbo].[sp_usuarios_select_by_email_and_scope]
(
	@Email NVARCHAR(50),
	@Scope NVARCHAR(20)
)
AS
BEGIN

	--Busca los datos del usuario y los almacena en una tabla temporal
	SELECT
		U.*,
		C.RazonSocial AS ClienteNombre
	INTO
		#TMP_USUARIO
	FROM
		[dbo].[Usuario] U WITH(NOLOCK)
		LEFT JOIN [$(DbMaster)].[dbo].[Cliente] C WITH(NOLOCK) ON C.ClienteId = U.ClienteId
	WHERE
		U.[Email] = @Email
		AND U.[Scope] = @Scope
		AND U.FechaHoraBaja IS NULL;


	--Devuelve Resultset con los datos del usuario
	SELECT
		*
	FROM
		#TMP_USUARIO;


	--Devuelve segundo Resultset con los permisos del usuario (si es que sigue activo)
	SELECT
		*
	FROM
		[dbo].[UsuarioPermiso] WITH(NOLOCK)
	WHERE
		[UsuarioId] = (SELECT TOP(1) [UsuarioId] FROM #TMP_USUARIO WHERE FechaHoraBaja IS NULL);


END