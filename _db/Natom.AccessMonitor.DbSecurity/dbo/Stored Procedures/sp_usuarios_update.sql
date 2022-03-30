CREATE PROCEDURE [dbo].[sp_usuarios_update]
(
	@UsuarioId INT,
	@Scope NVARCHAR(20),
	@Nombre NVARCHAR(50),
	@Apellido NVARCHAR(50),
	@PermisosId AS [dbo].[ID_char50_list] READONLY,
	@UpdatedByUsuarioId INT
)
AS
BEGIN

	BEGIN TRANSACTION

		--Actualizamos los datos del usuario
		UPDATE [dbo].[Usuario]
			SET Nombre = @Nombre,
				Apellido = @Apellido
			WHERE
				UsuarioId = @UsuarioId
				AND FechaHoraBaja IS NULL;

		--Si no encontro al usuario, devolvemos que el usuario no existe!
		IF @@ROWCOUNT = 0
			RETURN -1;


		--Consultamos los permisos actuales
		SELECT
			UP.UsuarioPermisoId,
			UP.PermisoId
		INTO
			#TMP_PERMISOS_ACTUALES
		FROM
			UsuarioPermiso UP WITH(NOLOCK)
			INNER JOIN Permiso P WITH(NOLOCK) ON P.PermisoId = UP.PermisoId
		WHERE
			UP.UsuarioId = @UsuarioId
			AND P.Scope = @Scope
		

		--Eliminamos permisos quitados
		DELETE FROM UsuarioPermiso
		WHERE
			UsuarioPermisoId IN
			(
				SELECT UsuarioPermisoId
				FROM #TMP_PERMISOS_ACTUALES
				WHERE PermisoId NOT IN (SELECT ID FROM @PermisosId)
			)


		--Insertamos permisos agregados
		INSERT INTO [dbo].[UsuarioPermiso]
			   ([UsuarioId]
			   ,[PermisoId]
			   ,[Scope])
		SELECT
			  @UsuarioId
			, ID
			, @Scope
		FROM
			@PermisosId
		WHERE ID NOT IN (SELECT PermisoId FROM #TMP_PERMISOS_ACTUALES);
		

		EXEC [$(DbMaster)].[dbo].[sp_history_insert] 0, 'Usuario', @UsuarioId, @UpdatedByUsuarioId, 'Edición';



	COMMIT TRANSACTION;



	
	RETURN 1;

END