CREATE PROCEDURE [dbo].[sp_history_insert]
(
	@ClientId INT,
	@EntityName NVARCHAR(30),
	@EntityId INT,
	@UsuarioId INT,
	@Action NVARCHAR(100)
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[zHistory_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']'') AND type in (N''U''))
			CREATE TABLE [dbo].[zHistory_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']
			(
				[HistoryId] INT NOT NULL IDENTITY(1,1),
				[DateTime] DATETIME NOT NULL,
				[EntityName] NVARCHAR(30) NOT NULL,
				[EntityId] INT,
				[UsuarioId] INT,
				[Action] NVARCHAR(100) NOT NULL,
				PRIMARY KEY (HistoryId)
			);

		

		INSERT INTO [dbo].[zHistory_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + ']
			(DateTime, EntityName, EntityId, UsuarioId, Action)
			VALUES
			(
				GETDATE(),
				''' + @EntityName + ''',
				' + (CASE WHEN @EntityId IS NULL THEN 'NULL' ELSE CAST(@EntityId AS nvarchar) END) + ',
				' + (CASE WHEN @UsuarioId IS NULL THEN 'NULL' ELSE CAST(@UsuarioId AS nvarchar) END) + ',
				''' + @Action + '''
			);
	';
	EXEC sp_executesql @SQL

END