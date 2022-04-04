CREATE PROCEDURE [dbo].[sp_usuarios_select_by_multiples_ids]
(
	@UsuariosIds AS [dbo].[ID_int_list] READONLY
)
AS
BEGIN

	SELECT
		*
	FROM
		Usuario WITH(NOLOCK)
	WHERE
		UsuarioId IN (SELECT ID FROM @UsuariosIds);
	

END