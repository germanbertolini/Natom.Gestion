CREATE PROCEDURE [dbo].[sp_token_select_by_usuarioid]
	@UsuarioId INT
AS
BEGIN

	SELECT
		*
	FROM
		[dbo].[Token] WITH(NOLOCK)
	WHERE
		UserId = @UsuarioId
		AND ExpiresAt > GETDATE()

END