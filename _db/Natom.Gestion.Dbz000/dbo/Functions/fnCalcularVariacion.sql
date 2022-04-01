
CREATE FUNCTION fnCalcularVariacion
(
	@Valor1 DECIMAL(18,2),
	@Valor2 DECIMAL(18,2)
) RETURNS DECIMAL(18,2)
AS
BEGIN

	DECLARE @Variacion DECIMAL(18,2) = 0

	IF @Valor1 > @Valor2
		BEGIN
			IF @Valor2 = 0
				SET @Valor2 = 1

			SET @Variacion = @Valor1 / @Valor2 * 100
		END
	ELSE IF @Valor2 > @Valor1
		BEGIN
			IF @Valor1 = 0
				SET @Valor1 = 1

			SET @Variacion = (@Valor2 / @Valor1 * 100) * -1
		END

	RETURN ROUND(@Variacion / 100, 2) --LO DIVIDO POR 100 PORQUE EL REPORT rdlc AL PORCENTAJE LO HACE * 100!!

END