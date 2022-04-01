CREATE PROCEDURE [dbo].[sp_token_delete]
	@TokenKey NVARCHAR(32),
	@Scope NVARCHAR(20)
AS
BEGIN

	DELETE FROM
		[dbo].[Token]
	WHERE
		[Key] = @TokenKey
		AND Scope = @Scope;

END