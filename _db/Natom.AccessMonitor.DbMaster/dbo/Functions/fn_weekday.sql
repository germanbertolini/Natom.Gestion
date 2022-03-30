CREATE FUNCTION [dbo].[fn_weekday]
(
	@Date AS DATE
)
RETURNS INT
AS
BEGIN

	DECLARE @Day INT = DATEPART(dw, @Date) - 1;

	IF @Day = 0
		SET @Day = 7

	RETURN @Day

END