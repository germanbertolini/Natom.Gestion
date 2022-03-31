using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Natom.Gestion.WebApp.Clientes.Backend.Filters;
using Natom.Gestion.WebApp.Clientes.Backend.Services;
using Natom.Gestion.WebApp.Clientes.Backend.Biz;
using Natom.Gestion.WebApp.Clientes.Backend.Biz.Services;

namespace Natom.Gestion.WebApp.Clientes.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BizDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")
            ));

            services.AddScoped<TransactionService>();

            services.AddSingleton<FeatureFlagsService>();

            services.AddHttpContextAccessor();

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(Configuration["CORS:FrontendOrigin"])
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
