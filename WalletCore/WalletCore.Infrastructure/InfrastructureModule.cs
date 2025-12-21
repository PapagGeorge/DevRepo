using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WalletCore.Application.Interfaces;
using WalletCore.Infrastructure.Configuration;
using WalletCore.Infrastructure.HttpClientInfrastructure;
using WalletCore.Infrastructure.Repositories;

namespace WalletCore.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
        {
            services.AddTransient<ExternalHttpLoggingHandler>();
            services.AddECBHttpClient();
            services.AddWalletDataServiceHttpClient();
            services.AddDbContexts();
            services.AddScoped<IExchangeRateMergeRepository, ExchangeRateMergeRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Optional: retry on failure
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddScoped<ICommandPublisher, CommandPublisher>();
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
