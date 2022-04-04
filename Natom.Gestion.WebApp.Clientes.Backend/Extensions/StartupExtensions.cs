using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Natom.Extensions.Auth.Entities;
using Natom.Extensions.Configuration.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natom.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddClientDbContextService(this IServiceCollection service)
        {
            service.AddScoped((serviceProvider) =>
            {
                var configurationService = serviceProvider.GetService<ConfigurationService>();
                var token = serviceProvider.GetService<AccessToken>();

                if (token == null)
                    return null;

                var connectionString = configurationService.GetValueAsync("ConnectionStrings.DbzXXX").GetAwaiter().GetResult();
                connectionString = connectionString.Replace("XXX", token.ClientId.ToString().PadLeft(3, '0'));

                var optionsBuilder = new DbContextOptionsBuilder<BizDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new BizDbContext(optionsBuilder.Options);
            });

            return service;
        }
    }
}
