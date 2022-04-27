using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Natom.Extensions.Common.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.DataTable;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.DTO.Negocio;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Model;
using Natom.Gestion.WebApp.Clientes.Backend.Entities.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class NegocioController : BaseController
    {
        private readonly FeatureFlagsService _featureFlagsService;

        public NegocioController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _featureFlagsService = (FeatureFlagsService)serviceProvider.GetService(typeof(FeatureFlagsService));
        }

        // GET: negocio/config
        [HttpGet]
        [ActionName("config")]
        public async Task<IActionResult> GetConfigAsync()
        {
            try
            {
                var manager = new NegocioManager(_serviceProvider);
                var config = manager.GetConfig();

                return Ok(new ApiResultDTO<NegocioConfigDTO>
                {
                    Success = true,
                    Data = new NegocioConfigDTO().From(config)
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // POST: negocio/config/save
        [HttpPost]
        [ActionName("config/save")]
        public async Task<IActionResult> PostSaveConfigAsync([FromBody] NegocioConfigDTO config)
        {
            try
            {
                var manager = new NegocioManager(_serviceProvider);
                await manager.SaveConfigAsync(config.ToModel());

                await RegistrarAccionAsync(1, nameof(NegocioConfig), "Actualización de datos");

                return Ok(new ApiResultDTO
                {
                    Success = true
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: negocio/app/config
        [HttpGet]
        [ActionName("app/config")]
        public async Task<IActionResult> GetAppConfigAsync()
        {
            try
            {
                var negocioConfigManager = new NegocioManager(_serviceProvider);
                var negocioConfig = negocioConfigManager.GetConfig();

                var appConfig = new AppConfigDTO();
                appConfig.FeatureFlags = _featureFlagsService.FeatureFlags;
                appConfig.NegocioConfig = new NegocioConfigDTO().From(negocioConfig);

                return Ok(new ApiResultDTO<AppConfigDTO>
                {
                    Success = true,
                    Data = appConfig
                });
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex);
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: negocio/logo
        [HttpGet]
        [ActionName("logo")]
        public async Task<IActionResult> GetLogoAsync(string clienteEncryptedId)
        {
            try
            {
                clienteEncryptedId = clienteEncryptedId.Split('\"').First(); //FIX BUG NETCORE REPORTING

                var clienteId = EncryptionService.Decrypt<int, Cliente>(Uri.UnescapeDataString(clienteEncryptedId));

                var connectionString = _configurationService.GetValueAsync("ConnectionStrings.DbzXXX").GetAwaiter().GetResult();
                connectionString = connectionString.Replace("XXX", clienteId.ToString().PadLeft(3, '0'));

                var optionsBuilder = new DbContextOptionsBuilder<BizDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                var db = new BizDbContext(optionsBuilder.Options);

                var negocioConfigManager = new NegocioManager(_serviceProvider);
                var negocioConfig = negocioConfigManager.GetCustomConfig(db);

                byte[] bytes = Convert.FromBase64String(negocioConfig.LogoBase64.Split(',').Last());
                Image image;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }

                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                var contentType = codecs.First(codec => codec.FormatID == image.RawFormat.Guid).MimeType;

                return File(bytes, contentType);
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex, new { clienteEncryptedId });
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }

        // GET: negocio/logo2/{clienteId}
        [HttpGet]
        [ActionName("logo2/{clienteId}")]
        public async Task<IActionResult> GetLogoAsync([FromRoute]int clienteId)
        {
            try
            {
                var connectionString = _configurationService.GetValueAsync("ConnectionStrings.DbzXXX").GetAwaiter().GetResult();
                connectionString = connectionString.Replace("XXX", clienteId.ToString().PadLeft(3, '0'));

                var optionsBuilder = new DbContextOptionsBuilder<BizDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                var db = new BizDbContext(optionsBuilder.Options);

                var negocioConfigManager = new NegocioManager(_serviceProvider);
                var negocioConfig = negocioConfigManager.GetCustomConfig(db);

                byte[] bytes = Convert.FromBase64String(negocioConfig.LogoBase64.Split(',').Last());
                Image image;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }

                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                var contentType = codecs.First(codec => codec.FormatID == image.RawFormat.Guid).MimeType;

                return File(bytes, contentType);
            }
            catch (HandledException ex)
            {
                return Ok(new ApiResultDTO { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _loggerService.LogException(_transaction.TraceTransactionId, ex, new { clienteId });
                return Ok(new ApiResultDTO { Success = false, Message = "Se ha producido un error interno." });
            }
        }
    }
}
