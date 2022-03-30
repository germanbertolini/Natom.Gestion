CREATE PROCEDURE [dbo].[sp_device_list_by_syncid]
(
	@SyncInstanceId NVARCHAR(32),
	@Search NVARCHAR(100) = NULL,
	@Skip INT,
	@Take INT
)
AS
BEGIN

	DECLARE @TotalRegistros INT = 0;
	DECLARE @TotalFiltrados INT = 0;
	
	SELECT
		Dev.Id AS Id,
		Dev.DeviceId AS DeviceId,
		Dev.DeviceName AS DeviceName,
		COALESCE(Dev.LastConfigurationAt, Sync.InstalledAt) AS DeviceLastConfigurationAt,
		Dev.SerialNumber AS DeviceSerialNumber,
		Dev.Model AS DeviceModel,
		Dev.Brand AS DeviceBrand,
		Dev.FirmwareVersion AS DeviceFirmwareVersion,
		COALESCE(G.[Name], '-Sin asignar-') AS GoalName,
		COALESCE(P.[Name], '-Sin asignar-') AS PlaceName
	INTO
		#TMP_SYNCHRONIZERS
	FROM
		[dbo].[Device] Dev WITH(NOLOCK)
		INNER JOIN [dbo].[Synchronizer] Sync WITH(NOLOCK) ON Dev.InstanceId = Sync.InstanceId
		LEFT JOIN [$(DbMaster)].[dbo].[Goal] G WITH(NOLOCK) ON G.GoalId = Dev.GoalId
		LEFT JOIN [$(DbMaster)].[dbo].[Place] P WITH(NOLOCK) ON P.PlaceId = G.PlaceId
	WHERE
		Dev.InstanceId = @SyncInstanceId;


	SET @TotalRegistros = COALESCE((SELECT COUNT(*) FROM #TMP_SYNCHRONIZERS), 0);

	SELECT
		*
	INTO
		#TMP_SYNCHRONIZERS_FILTRADOS
	FROM
		#TMP_SYNCHRONIZERS
	WHERE
		@Search IS NULL
		OR
		(
			@Search IS NOT NULL
			AND
			(
				DeviceId LIKE '%' + @Search + '%'
				OR DeviceName LIKE '%' + @Search + '%'
				OR DeviceModel LIKE '%' + @Search + '%'
				OR DeviceBrand LIKE '%' + @Search + '%'
			)
		);

	SET @TotalFiltrados = COALESCE((SELECT COUNT(*) FROM #TMP_SYNCHRONIZERS_FILTRADOS), 0);


	SELECT
		F.*,
		@TotalFiltrados AS TotalFiltrados,
		@TotalRegistros AS TotalRegistros
	FROM
		#TMP_SYNCHRONIZERS_FILTRADOS F
	ORDER BY
		F.DeviceId ASC
	OFFSET @Skip ROWS
	FETCH NEXT @Take ROWS ONLY;

END