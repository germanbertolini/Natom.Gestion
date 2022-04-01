CREATE PROCEDURE [dbo].[sp_token_select_by_userid_and_scope]
	@UserId INT,
	@Scope NVARCHAR(20)
AS
BEGIN

	SELECT TOP 1
		*
	FROM
		[dbo].[Token] WITH(NOLOCK)
	WHERE
		UserId = @UserId
		AND Scope = @Scope
	ORDER BY
		ExpiresAt DESC;

END