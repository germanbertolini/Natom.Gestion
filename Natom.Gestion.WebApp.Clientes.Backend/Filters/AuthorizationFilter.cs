using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using Natom.Gestion.WebApp.Clientes.Backend.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Entities;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Exceptions;
using Natom.Gestion.WebApp.Clientes.Backend.Biz;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Services;

namespace Natom.Gestion.WebApp.Clientes.Backend.Filters
{
    public class AuthorizationFilter : ActionFilterAttribute, IAsyncAuthorizationFilter
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly TransactionService _transaction;
        private readonly FeatureFlagsService _featureFlagsService;
        private readonly BizDbContext _db;

        private string _controller = null;
        private string _action = null;

        public AuthorizationFilter(IServiceProvider serviceProvider)
        {
            _transaction = (TransactionService)serviceProvider.GetService(typeof(TransactionService));
            _featureFlagsService = (FeatureFlagsService)serviceProvider.GetService(typeof(FeatureFlagsService));
            _accessor = (IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor));
            _db = (BizDbContext)serviceProvider.GetService(typeof(BizDbContext));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            Token token = null;

            this.Init(context);
            try
            {
                if (!(_controller.Equals("auth") || (_controller.Equals("users") && _action.Equals("confirm"))))
                {
                    IEnumerable<string> headerValuesForAuthorization = context.HttpContext.Request.Headers["Authorization"];
                    string cookieValueForAuthorization = context.HttpContext.Request.Cookies["Authorization"];
                    if ((headerValuesForAuthorization == null || headerValuesForAuthorization.Count() == 0) && string.IsNullOrEmpty(cookieValueForAuthorization)) throw new HandledException("Falta el 'Authorization' header en el Request.");

                    //SI EL AUTHORIZATION NO VINO EN EL HEADER ENTONCES LO TOMAMOS DE LA COOKIE
                    string authorization = string.IsNullOrEmpty(cookieValueForAuthorization)
                                                ? headerValuesForAuthorization.ToString()
                                                : cookieValueForAuthorization;

                    if (!authorization.StartsWith("Bearer")) throw new HandledException("'Authorization' header inválido.");
                    
                    authorization = authorization.Replace("Bearer ", "");

                    token = OAuthService.Decode(authorization);
                    var expirationTime = OAuthService.UnixTimeStampToDateTime(token.ExpirationTime);
                    if (expirationTime < DateTime.Now)
                        throw new HandledException("Token expirado.");

                    //FUERA DE HORARIO LABORAL SOLO EL ADMIN PUEDE OPERAR
                    if (_featureFlagsService.FeatureFlags.Acceso.RestringirPorHorario)
                        if (token.UserId != 0 && (DateTime.Now.Hour >= _featureFlagsService.FeatureFlags.Acceso.RangoHorarioPermitidoHasta || DateTime.Now.Hour <= _featureFlagsService.FeatureFlags.Acceso.RangoHorarioPermitidoDesde))
                            throw new HandledException("Acceso denegado.");

                    //START LOG
                    _transaction.Token = token;
                }
            }
            catch (HandledException ex)
            {
                string err = ex.Message;
                context.HttpContext.Response.StatusCode = 403;
                context.Result = new JsonResult(new { message = err });
            }
            catch (Exception ex)
            {
                LoggingService.LogExceptionAsync(_db, ex, (int?)_transaction?.Token?.UserId);
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult(new { error = "Se ha producido un error interno al autenticar." });
            }
        }

        private string GetLangFromHeader(AuthorizationFilterContext context)
        {
            IEnumerable<string> headerValuesForLang = context.HttpContext.Request.Headers["Lang"];
            return (headerValuesForLang == null || headerValuesForLang.Count() == 0) 
                        ? "EN"
                        : headerValuesForLang.ToString().ToUpper();
        }

        private void Init(AuthorizationFilterContext context)
        {
            var lang = GetLangFromHeader(context);
            var actionDescriptor = ((ControllerActionDescriptor)context.ActionDescriptor);
            var ip = GetRealIP(context);
            var agent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            _controller = actionDescriptor.RouteValues["controller"].ToLower();
            _action = actionDescriptor.RouteValues["action"].ToLower();
        }

        private string GetRealIP(AuthorizationFilterContext context)
        {
            var forwardedFor = context.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"];

            var userIpAddress = forwardedFor.Count() == 0 || String.IsNullOrWhiteSpace(forwardedFor.ToString())
                                    ? _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                                    : forwardedFor.ToString().Split(',').Select(s => s.Trim()).FirstOrDefault();

            return userIpAddress;
        }
    }
}
