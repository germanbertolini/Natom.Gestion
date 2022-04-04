using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Natom.Extensions.Auth.Entities;
using Natom.Extensions.Configuration.Services;
using Natom.Extensions.Logger.Entities;
using Natom.Extensions.Logger.Services;
using Natom.Extensions.Mailer.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Biz;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Managers;
using Natom.Gestion.WebApp.Clientes.Backend.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IConfiguration _configuration;
        protected readonly IWebHostEnvironment _hostingEnvironment;

        protected readonly ConfigurationService _configurationService;
        protected readonly LoggerService _loggerService;
        protected readonly Transaction _transaction;
        protected readonly MailService _mailService;
        protected readonly AccessToken _accessToken;

        protected BizDbContext _db;
        protected string _userAgent;

        protected Task RegistrarAccionAsync(int entityId, string entityName, string accion, string motivo = null)
                        => new BaseManager(_serviceProvider).RegistrarEnHistoricoCambiosAsync(entityId, entityName, accion, (int?)_accessToken?.UserId ?? 0, motivo);

        public BaseController(IServiceProvider serviceProvider)
        {
            var httpContextAccessor = (IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor));
            _userAgent = httpContextAccessor.HttpContext.Request.Headers["User-Agent"];

            _configuration = (IConfiguration)serviceProvider.GetService(typeof(IConfiguration));
            _hostingEnvironment = (IWebHostEnvironment)serviceProvider.GetService(typeof(IWebHostEnvironment));

            _serviceProvider = serviceProvider;

            _db = (BizDbContext)serviceProvider.GetService(typeof(BizDbContext));

            _configurationService = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));

            _accessToken = (AccessToken)serviceProvider.GetService(typeof(AccessToken));
            _transaction = (Transaction)serviceProvider.GetService(typeof(Transaction));
            _mailService = (MailService)serviceProvider.GetService(typeof(MailService));
        }

        protected string GetAuthorizationFromHeader()
        {
            string authorization = null;
            StringValues stringValues;
            if (Request.Headers.TryGetValue("Authorization", out stringValues))
                authorization = stringValues.FirstOrDefault();
            return authorization;
        }
    }
}
