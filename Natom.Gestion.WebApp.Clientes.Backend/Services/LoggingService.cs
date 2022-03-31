using Natom.Gestion.WebApp.Clientes.Backend.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Clientes.Backend.Services
{
    public class LoggingService
    {
        public static Task LogExceptionAsync(BizDbContext db, Exception ex, int? usuarioId = null, string userAgent = null)
        {
            db.Logs.Add(new Entities.Model.Log
            {
                FechaHora = DateTime.Now,
                UserAgent = userAgent,
                UsuarioId = usuarioId,
                Exception = ex.ToString()
            });
            return db.SaveChangesAsync();
        }
    }
}
