using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WalletCore.Application.Interfaces;
using WalletCore.Infrastructure.Configuration;
using WalletCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WalletCore.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
        {
            services.AddDbContexts();
            services.AddScoped<IExchangeRateRepository, ExchangeRateMergeRepository>();
            return services;
        }

        private static void AddDbContexts(this IServiceCollection services)
        {
            services.AddDbContext<ExchangeRateDbContext>((serviceProvider, options) =>
            {
                var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
                options.UseSqlServer(dbOptions.ExchangeRateDb, sql =>
                {
                    sql.MigrationsAssembly(typeof(ExchangeRateDbContext).Assembly.FullName);
                });
            });
        }
    }
}
