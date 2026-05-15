using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TransactionSimulator.Domain.Interfaces;

namespace TransactionSimulator.Application
{
    public static class DependencyInjection1
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITransactionService, TransactionService>();
            return services;
        }
    }
}
