CREATE PROCEDURE [dbo].[sp_usuarios_confirmar]
(
	@Secret CHAR(32),
	@ClaveMD5 CHAR(32)
)
AS
BEGIN

	UPDATE [dbo].[Usuario]
		SET SecretConfirmacion = NULL,
			FechaHoraConfirmacionEmail = GETDATE(),
			FechaHoraUltimoEmailEnviado = NULL,
			Clave = @ClaveMD5
		WHERE SecretConfirmacion = @Secret
			AND FechaHoraBaja IS NULL;
	
	RETURN @@ROWCOUNT;

END