using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
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
        protected IServiceProvider _serviceProvider;
        protected IConfiguration _configuration;
        protected IWebHostEnvironment _hostingEnvironment;
        protected Token _token;
        protected BizDbContext _db;
        protected string _userAgent;

        protected Task RegistrarAccionAsync(int entityId, string entityName, string accion, string motivo = null)
                        => new BaseManager(_serviceProvider).RegistrarEnHistoricoCambiosAsync(entityId, entityName, accion, (int?)_token?.UserId ?? 0, motivo);

        public BaseController(IServiceProvider serviceProvider)
        {
            var httpContextAccessor = (IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor));
            _serviceProvider = serviceProvider;
            _db = (BizDbContext)serviceProvider.GetService(typeof(BizDbContext));
            _configuration = (IConfiguration)serviceProvider.GetService(typeof(IConfiguration));
            _hostingEnvironment = (IWebHostEnvironment)serviceProvider.GetService(typeof(IWebHostEnvironment));
            _token = (Token)serviceProvider.GetService(typeof(Token));
            _userAgent = httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
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
