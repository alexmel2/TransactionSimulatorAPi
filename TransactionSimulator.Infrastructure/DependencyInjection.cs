using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionSimulator.Domain.Interfaces;
using TransactionSimulator.Infrastructure.Caching;

namespace TransactionSimulator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            ArgumentNullException.ThrowIfNull(connectionString, "Connection string 'DefaultConnection' not found in configuration.");
            services.AddScoped<IApplicationDbContext, AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                      options.UseSqlServer(
                     connectionString,
                      b => b.MigrationsAssembly("TransactionSimulator.Infrastructure")));
            services.AddMemoryCache();
            services.AddHostedService<RegionCacheService>();
            return services;
        }
    }
}
