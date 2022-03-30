using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Natom.Gestion.Core.Biz.Managers;
using Natom.Extensions.Auth.Entities;
using Natom.Extensions.Configuration.Services;
using Natom.Extensions.Logger.Entities;
using Natom.Extensions.Logger.Services;
using Natom.Extensions.Mailer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ConfigurationService _configurationService;
        protected readonly LoggerService _loggerService;
        protected readonly Transaction _transaction;
        protected readonly MailService _mailService;
        protected readonly AccessToken _accessToken;

        protected Task RegistrarAccionAsync(int clienteId, int entityId, string entityName, string accion)
                        => new BaseManager(_serviceProvider).RegistrarEnHistoricoCambiosAsync(clienteId, entityId, entityName, accion, (int?)_transaction?.UserId ?? 0);

        public BaseController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

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
