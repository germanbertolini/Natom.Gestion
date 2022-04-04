using Natom.Extensions.Common.Exceptions;
using Natom.Extensions.Configuration.Services;
using Natom.Gestion.WebApp.Admin.Backend.Biz.Entities.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Managers
{
    public class DacpacManager : BaseManager
    {
        private readonly ConfigurationService _configurationService;

        public DacpacManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _configurationService = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
        }

        public async Task<string> DeployNewDacpacAsync(Cliente cliente)
        {
            var sqlpackageExePath = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.SqlPackageEXE.Path");
            var dacpacPath = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.DACPAC.Path");
            var newDatabaseName = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.NewDB.Name");
            var targetServerName = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.TargetServerName");

            newDatabaseName = newDatabaseName.Replace("XXX", cliente.ClienteId.ToString().PadLeft(3, '0'));

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.FileName = sqlpackageExePath;
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            processStartInfo.Arguments = $"/Action:Publish /SourceFile:\"{dacpacPath}\" /TargetDatabaseName:\"{newDatabaseName}\" /TargetServerName:\"{targetServerName}\"";

            var p = new Process();
            p.StartInfo = processStartInfo;

            var outputBuilder = new StringBuilder();
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    outputBuilder.Append(e.Data);
                }
            );

            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.CancelOutputRead();

            var result = outputBuilder.ToString();

            if (!result.Contains("Successfully published database"))
                throw new Exception($"No se pudo desplegar la nueva base de datos '{newDatabaseName}': {result}");
            else
            {
                var successInfo = result.Split("(Complete)").Last();
                return successInfo;
            }

        }

        public async Task CheckDeploymentParamsAsync()
        {
            var sqlpackageExePath = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.SqlPackageEXE.Path");
            var dacpacPath = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.DACPAC.Path");
            var newDatabaseName = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.NewDB.Name");
            var targetServerName = await _configurationService.GetValueAsync("WebApp.Admin.NewDeployment.TargetServerName");

            if (string.IsNullOrEmpty(sqlpackageExePath))
                throw new HandledException("Falta definir 'WebApp.Admin.NewDeployment.SqlPackageEXE.Path' en la configuración");

            if (string.IsNullOrEmpty(dacpacPath))
                throw new HandledException("Falta definir 'WebApp.Admin.NewDeployment.DACPAC.Path' en la configuración");

            if (string.IsNullOrEmpty(newDatabaseName))
                throw new HandledException("Falta definir 'WebApp.Admin.NewDeployment.NewDB.Name' en la configuración");

            if (string.IsNullOrEmpty(targetServerName))
                throw new HandledException("Falta definir 'WebApp.Admin.NewDeployment.TargetServerName' en la configuración");

            if (!File.Exists(sqlpackageExePath))
                throw new HandledException("No existe 'sqlpackage.exe' en la dirección especificada en la configuración");

            if (!File.Exists(dacpacPath))
                throw new HandledException("No existe el .dacpac en la dirección especificada en la configuración");

        }
    }
}
