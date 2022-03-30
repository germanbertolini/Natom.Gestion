CREATE PROCEDURE [dbo].[sp_movements_prepare_turnos]
(
	@ClientId INT
)
AS
BEGIN

	--PRIMERO CREAMOS LA TABLA SI NO EXISTE
	DECLARE @SQL NVARCHAR(4000)
	SET @SQL = '
		IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''[dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed]'') AND type in (N''U''))
		BEGIN

			DECLARE @LastTurnoRun AS DATETIME = (SELECT MovementProcess_LastTurnosRun FROM [dbo].[Cliente] WITH(NOLOCK) WHERE ClienteId = ' + CAST(@ClientId AS VARCHAR) + ')
			IF @LastTurnoRun IS NULL OR (@LastTurnoRun IS NOT NULL AND DATEADD(MINUTE, 30, @LastTurnoRun) <= GETDATE())
			BEGIN
				INSERT INTO [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed]
					   ([Date]
					   ,[DocketNumber]
					   ,[DocketId]
					   ,[ExpectedIn]
					   ,[ExpectedOut]
					   ,[ExpectedPlaceId])
				SELECT
					T.[Date],
					T.DocketNumber,
					T.DocketId,
					CAST(T.[From] AS TIME),
					CAST(T.[To] AS TIME),
					T.PlaceId
				FROM 
					[dbo].[fn_current_turnos] (' + CAST(@ClientId AS VARCHAR) + ', NULL, 40) T
					LEFT JOIN [dbo].[zMovement_Client' + REPLACE(STR(CAST(@ClientId AS VARCHAR), 3), SPACE(1), '0') + '_Processed] M WITH(NOLOCK)
						ON	M.[Date] = T.[Date]
							AND T.DocketId = M.DocketId
							AND CAST(T.[From] AS TIME) = M.[ExpectedIn]
							AND CAST(T.[To] AS TIME) = M.[ExpectedOut]
				WHERE
					M.ExpectedIn IS NULL;

				UPDATE [dbo].[Cliente] WITH(READPAST)
					SET MovementProcess_LastTurnosRun = GETDATE()
					WHERE ClienteId = ' + CAST(@ClientId AS VARCHAR) + ';

			END

		END';

	EXEC sp_executesql @SQL

END