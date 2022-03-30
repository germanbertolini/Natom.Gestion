﻿CREATE PROCEDURE [dbo].[sp_usuarios_recover_by_email]
(
	@Scope NVARCHAR(20),
	@Email NVARCHAR(50),
	@SecretConfirmation CHAR(32),
	@ByUsuarioId INT
)
AS
BEGIN

	DECLARE @FechaHoraUltimoEmail DATETIME = NULL;
	DECLARE @UsuarioId INT = 0;
	DECLARE @Result INT = 0;
	   
	BEGIN TRANSACTION

		SELECT
			@UsuarioId = UsuarioId,
			@FechaHoraUltimoEmail = FechaHoraUltimoEmailEnviado
		FROM
			[dbo].[Usuario] WITH(NOLOCK)
		WHERE
			[Scope] = @Scope
			AND [Email] = @Email
			AND [FechaHoraBaja] IS NULL;


		IF @UsuarioId <> 0
		BEGIN

			IF (@FechaHoraUltimoEmail IS NULL OR (@FechaHoraUltimoEmail IS NOT NULL AND DATEADD(MINUTE, 10, @FechaHoraUltimoEmail) < GETDATE()))
				BEGIN
					UPDATE [dbo].[Usuario]
						SET 
						   [SecretConfirmacion] = @SecretConfirmation,
						   [FechaHoraConfirmacionEmail] = NULL,
						   [FechaHoraUltimoEmailEnviado] = GETDATE(),
						   [Clave] = NULL
						WHERE
							UsuarioId = @UsuarioId;

					SET @Result = @@ROWCOUNT;
				END
			ELSE
				SET @Result = 99

			IF @Result = 1
				EXEC [$(DbMaster)].[dbo].[sp_history_insert] 0, 'Usuario', @UsuarioId, @ByUsuarioId, 'Envio mail de recuperación de clave (solicitado por el mismo)';
		END


	COMMIT TRANSACTION;
	   
	
	IF @Result > 0
		SELECT
			[UsuarioId],
			[Scope],
			[ClienteId],
			[Nombre],
			[Apellido],
			[Email],
			[Clave],
			[FechaHoraConfirmacionEmail],
			@FechaHoraUltimoEmail AS [FechaHoraUltimoEmailEnviado],
			[SecretConfirmacion],
			[FechaHoraAlta],
			[FechaHoraBaja]
		FROM
			[dbo].[Usuario] WITH(NOLOCK)
		WHERE UsuarioId = @UsuarioId;

END