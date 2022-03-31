using Microsoft.AspNetCore.Mvc.Filters;
using Natom.Gestion.WebApp.Clientes.Backend.Services;
using System;

namespace Natom.Gestion.WebApp.Clientes.Backend.Filters
{
    public class ResultFilter : IResultFilter
    {
        private readonly TransactionService _transaction;

        public ResultFilter(IServiceProvider serviceProvider)
        {
            _transaction = (TransactionService)serviceProvider.GetService(typeof(TransactionService));
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            //_traceService.Log(_transactionService.CurrentTrace.TraceTransactionId, eTraceLogType.INFO, "Fin de transacción");
            //_traceService.FinishTransaction(_transactionService.CurrentTrace);
            //_bizDbService.Close();
        }
    }
}
