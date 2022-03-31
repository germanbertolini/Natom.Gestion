using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Natom.Gestion.WebApp.Admin.Backend.Biz.PackageConfig;
using Natom.Extensions.Configuration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natom.Gestion.WebApp.Admin.Backend.Biz.Extensions
{
    public static class DapperExtensions
    {
        private static bool _hostedServiceAdded = false;

        public static IServiceCollection AddCoreBiz(this IServiceCollection service, string scope)
        {
            service.AddSingleton(new CoreBizConfig { Scope = scope });

            service.AddDbContext<MasterDbContext>((serviceProvider, options) => {
                var configuration = (ConfigurationService)serviceProvider.GetService(typeof(ConfigurationService));
                var connectionString = configuration.GetValueAsync("ConnectionStrings.DbMaster").GetAwaiter().GetResult();
                options.UseSqlServer(connectionString);
            });

            return service;
        }
    }
}
