CREATE PROCEDURE [dbo].[sp_device_list_by_clientid]
(
	@ClientId INT,
	@Search NVARCHAR(100) = NULL,
	@Skip INT,
	@Take INT
)
AS
BEGIN

	DECLARE @TotalRegistros INT = 0;
	DECLARE @TotalFiltrados INT = 0;
	
	SELECT
		*,

		--El status de si está Online o no el Reloj se toma en base a la ultima vez que sincronizó el Reloj
		CAST(CASE WHEN DATEADD(MINUTE,  R.CurrentSyncToServerMinutes + 5, COALESCE(R.LastSyncAt, R.LastConnectionAt)) > GETDATE() THEN 1 ELSE 0 END AS BIT) AS IsOnline
	INTO
		#TMP_DEVICES
	FROM
	(
		SELECT
			D.Id,
			D.InstanceId,
			D.DeviceId,
			D.DeviceName,
			D.DeviceIP,
			COALESCE(G.[Name], '-Sin asignar-') AS Goal,
			G.Lat AS Lat,
			G.Lng AS Lng,
			COALESCE(P.[Name], '-Sin asignar-') AS Place,
			D.AddedAt,
			D.SerialNumber,
			D.Model,
			D.Brand,
			S.InstallationAlias AS SyncName,
			S.CurrentSyncToServerMinutes,
			CASE WHEN DATEADD(MINUTE, S.CurrentSyncToServerMinutes, S.LastSyncAt) < DATEADD(MINUTE, -5, GETDATE()) THEN
				NULL
			ELSE
				DATEADD(MINUTE, S.CurrentSyncToServerMinutes, S.LastSyncAt)
			END AS NextSyncAt, --NextSyncAt es tomando la ultima sincronización pero del Sincronizador contra el servidor
			S.LastConnectionAt,
			D.LastSyncAt --Este es la ultima sincronización del Reloj
		FROM
			[dbo].[Device] D WITH(NOLOCK)
			INNER JOIN [dbo].[Synchronizer] S WITH(NOLOCK) ON S.InstanceId = D.InstanceId
			LEFT JOIN [$(DbMaster)].[dbo].[Goal] G WITH(NOLOCK) ON G.GoalId = D.GoalId
			LEFT JOIN [$(DbMaster)].[dbo].[Place] P WITH(NOLOCK) ON P.PlaceId = G.PlaceId
		WHERE
			S.ClientId = @ClientId
			AND S.RemovedAt IS NULL
	) R


	SET @TotalRegistros = COALESCE((SELECT COUNT(*) FROM #TMP_DEVICES), 0);

	SELECT
		*
	INTO
		#TMP_DEVICES_FILTRADOS
	FROM
		#TMP_DEVICES
	WHERE
		@Search IS NULL
		OR
		(
			@Search IS NOT NULL
			AND
			(
				DeviceId LIKE '%' + @Search + '%'
				OR DeviceName LIKE '%' + @Search + '%'
				OR DeviceIP LIKE '%' + @Search + '%'
				OR Goal LIKE '%' + @Search + '%'
				OR Place LIKE '%' + @Search + '%'
				OR Brand LIKE '%' + @Search + '%'
				OR Model LIKE '%' + @Search + '%'
				OR SyncName LIKE '%' + @Search + '%'
			)
		);

	SET @TotalFiltrados = COALESCE((SELECT COUNT(*) FROM #TMP_DEVICES_FILTRADOS), 0);


	SELECT
		F.*,
		@TotalFiltrados AS TotalFiltrados,
		@TotalRegistros AS TotalRegistros
	FROM
		#TMP_DEVICES_FILTRADOS F
	ORDER BY
		F.DeviceId ASC
	OFFSET @Skip ROWS
	FETCH NEXT @Take ROWS ONLY;

END