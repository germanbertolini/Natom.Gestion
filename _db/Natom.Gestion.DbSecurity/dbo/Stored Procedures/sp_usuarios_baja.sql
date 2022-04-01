CREATE PROCEDURE [dbo].[sp_usuarios_baja]
(
	@UsuarioId INT,
	@BajaByUsuarioId INT
)
AS
BEGIN

	UPDATE [dbo].[Usuario]
		SET FechaHoraBaja = GETDATE()
		WHERE UsuarioId = @UsuarioId;
	
		

	--EXEC [$(DbMaster)].[dbo].[sp_history_insert] 0, 'Usuario', @UsuarioId, @BajaByUsuarioId, 'Baja';

END