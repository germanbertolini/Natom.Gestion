CREATE PROCEDURE [dbo].[sp_usuarios_create]
(
	@Scope NVARCHAR(20),
	@Nombre NVARCHAR(50),
	@Apellido NVARCHAR(50),
	@Email NVARCHAR(50),
	@SecretConfirmation CHAR(32),
	@ClienteId INT = NULL,
	@PermisosId AS [dbo].[ID_char50_list] READONLY,

	@CreatedByUsuarioId INT
)
AS
BEGIN

	DECLARE @UsuarioId INT = 0;

	--Verificamos que no exista el usuario
	IF EXISTS(SELECT * FROM [dbo].[Usuario] WHERE Email = @Email AND Scope = @Scope AND FechaHoraBaja IS NULL)
		RETURN -1;



	BEGIN TRANSACTION

		INSERT INTO [dbo].[Usuario]
			   ([Scope]
			   ,[Nombre]
			   ,[Apellido]
			   ,[Email]
			   ,[Clave]
			   ,[FechaHoraConfirmacionEmail]
			   ,[FechaHoraUltimoEmailEnviado]
			   ,[SecretConfirmacion]
			   ,[FechaHoraAlta]
			   ,[FechaHoraBaja]
			   ,[ClienteId])
		 VALUES
		 (
				@Scope
			   ,@Nombre
			   ,@Apellido
			   ,@Email
			   ,NULL								--@ClaveMD5
			   ,NULL								--@FechaHoraConfirmacionEmail
			   ,GETDATE()							--FechaHoraUltimoEmailEnviado
			   ,@SecretConfirmation					--@SecretConfirmacion
			   ,GETDATE()							--@FechaHoraAlta
			   ,NULL								--@FechaHoraBaja
			   ,COALESCE(@ClienteId, 0)
		 );

		SET @UsuarioId = @@IDENTITY;


		INSERT INTO [dbo].[UsuarioPermiso]
			   ([UsuarioId]
			   ,[PermisoId]
			   ,[Scope])
		SELECT
			  @UsuarioId
			, ID
			, @Scope
		FROM
			@PermisosId;


		--EXEC [$(DbMaster)].[dbo].[sp_history_insert] 0, 'Usuario', @UsuarioId, @CreatedByUsuarioId, 'Alta';


	COMMIT TRANSACTION;



	
	RETURN @UsuarioId;

END