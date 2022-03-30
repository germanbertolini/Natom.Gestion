CREATE PROCEDURE [dbo].[sp_token_delete_by_clientid_and_scope]
	@ClientId INT,
	@Scope NVARCHAR(20)
AS
BEGIN

	DELETE FROM
		[dbo].[Token]
	WHERE
		Scope = @Scope
		AND ClientId = @ClientId
		AND ExpiresAt > GETDATE()

END