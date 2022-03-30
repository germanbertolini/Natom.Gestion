CREATE PROCEDURE [dbo].[sp_synchronizers_list_by_cliente]
(
	@ClienteId INT = NULL,
	@Search NVARCHAR(100) = NULL,
	@Skip INT,
	@Take INT
)
AS
BEGIN

	DECLARE @TotalRegistros INT = 0;
	DECLARE @TotalFiltrados INT = 0;
	
	SELECT
		Sync.InstanceId,
		Sync.InstallationAlias,
		Sync.InstallerName,
		Sync.InstalledAt,
		Sync.ActivatedAt,
		CAST(CASE WHEN Sync.RemovedAt IS NULL THEN 0 ELSE 1 END AS BIT) AS Removed,
		Sync.LastSyncAt,
		SUM(CASE WHEN Dev.DeviceId IS NULL THEN 0 ELSE 1 END) AS DevicesCount
	INTO
		#TMP_SYNCHRONIZERS
	FROM
		[dbo].[Synchronizer] Sync WITH(NOLOCK)
		LEFT JOIN [dbo].[Device] Dev WITH(NOLOCK) ON Dev.InstanceId = Sync.InstanceId
	WHERE
		ClientId = @ClienteId
	GROUP BY
		Sync.InstanceId,
		Sync.InstallationAlias,
		Sync.InstallerName,
		Sync.InstalledAt,
		Sync.ActivatedAt,
		Sync.RemovedAt,
		Sync.LastSyncAt;


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
				InstallationAlias LIKE '%' + @Search + '%'
				OR InstallerName LIKE '%' + @Search + '%'
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
		F.InstallationAlias ASC
	OFFSET @Skip ROWS
	FETCH NEXT @Take ROWS ONLY;

END