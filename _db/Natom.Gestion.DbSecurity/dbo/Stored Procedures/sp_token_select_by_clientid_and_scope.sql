CREATE PROCEDURE [dbo].[sp_token_select_by_clientid_and_scope]
	@ClientId INT,
	@Scope NVARCHAR(20)
AS
BEGIN

	SELECT
		*
	FROM
		[dbo].[Token] WITH(NOLOCK)
	WHERE
		Scope = @Scope
		AND ClientId = @ClientId
		AND ExpiresAt > GETDATE()

END