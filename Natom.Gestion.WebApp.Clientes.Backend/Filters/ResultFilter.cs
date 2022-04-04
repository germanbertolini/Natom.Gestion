using Microsoft.AspNetCore.Mvc.Filters;
using Natom.Extensions.Auth.Entities;
using Natom.Extensions.Logger.Entities;
using Natom.Extensions.Logger.Services;
using System;

namespace Natom.Gestion.WebApp.Clientes.Backend.Filters
{
    public class ResultFilter : IResultFilter
    {
        private readonly LoggerService _loggerService;
        private readonly Transaction _transaction;
        private readonly AccessToken _accessToken;

        public ResultFilter(IServiceProvider serviceProvider)
        {
            _loggerService = (LoggerService)serviceProvider.GetService(typeof(LoggerService));
            _transaction = (Transaction)serviceProvider.GetService(typeof(Transaction));
            _accessToken = (AccessToken)serviceProvider.GetService(typeof(AccessToken));
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {

        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _loggerService.LogInfo(_transaction.TraceTransactionId, "Fin transacción");
        }
    }
}
