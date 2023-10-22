using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace WebShop.Infrastructure
{
    public static class DepedancyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("Default"),
                    ctxOptions =>
                    {
                        // ctxOptions.MaxBatchSize(1000);
                        //  ctxOptions.EnableRetryOnFailure();
                        ctxOptions.MigrationsAssembly("WebShop.Infrastructure");
                        //ctxOptions.UseNetTopologySuite();
                        ctxOptions.CommandTimeout(180);
                    }
                ).EnableSensitiveDataLogging();
            });

            return services;
        }

        public static void UseProjectSqlServer(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
