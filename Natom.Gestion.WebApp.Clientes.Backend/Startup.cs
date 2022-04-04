using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Natom.Gestion.WebApp.Clientes.Backend.Filters;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Services;
using Natom.Extensions.Configuration.Services;
using Natom.Extensions.Auth.Services;
using Natom.Extensions;

namespace Natom.Gestion.WebApp.Clientes.Backend
{
    public class Startup
    {
        private string _frontEndAddress;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddHttpContextAccessor()
                .AddSingleton<FeatureFlagsService>()
                .AddClientDbContextService()
                .AddConfigurationService(refreshTimeMS: 30000)
                .AddCacheService()
                .AddAuthService(scope: "WebApp.Clientes")
                .AddLoggerService(systemName: "WebApp.Clientes", insertEachMS: 30000, bulkInsertSize: 10000)
                .AddMailService();


            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(_frontEndAddress)
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services
                .AddControllers(options => {
                    options.Filters.Add(typeof(AuthorizationFilter));
                    options.Filters.Add(typeof(ResultFilter));
                })
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ConfigurationService configurationService, AuthService authService)
        {
            _frontEndAddress = configurationService.GetValueAsync("WebApp.Clientes.URL").GetAwaiter().GetResult();
            if (_frontEndAddress.EndsWith('/'))
                _frontEndAddress = _frontEndAddress.Substring(0, _frontEndAddress.Length - 1);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
