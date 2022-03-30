CREATE PROCEDURE [dbo].[sp_token_delete_by_usuarioid]
	@UsuarioId INT
AS
BEGIN

	DELETE FROM
		[dbo].[Token]
	WHERE
		UserId = @UsuarioId
		AND ExpiresAt > GETDATE()

END