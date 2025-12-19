using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WalletCore.DataService.Infrastructure.Configuration;
using WalletCore.DataService.Infrastructure.Interfaces;
using WalletCore.DataService.Repositories.Repositorues;

namespace WalletCore.DataService.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
        {
            services.AddDbContexts();
            services.AddScoped<IExchangeRateMergeRepository, ExchangeRateMergeRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            return services;
        }

        private static void AddDbContexts(this IServiceCollection services)
        {
            services.AddDbContext<WalletDbContext>((serviceProvider, options) =>
            {
                var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
                options.UseSqlServer(dbOptions.ExchangeRateDb, sql =>
                {
                    sql.MigrationsAssembly(typeof(WalletDbContext).Assembly.FullName);
                });
            });
        }
    }
}
