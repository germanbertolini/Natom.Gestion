CREATE PROCEDURE [dbo].[sp_clientes_select_all]
AS
BEGIN

	SELECT
		*
	FROM
		[dbo].[Cliente] WITH(NOLOCK)

END