using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WalletCore.DataService.Infrastructure.Configuration;
using WalletCore.DataService.Infrastructure.Consumers;
using WalletCore.DataService.Infrastructure.Interfaces;
using WalletCore.DataService.Repositories.Repositorues;
using WalletCore.DataService.Services.Infrastructure;
using WalletCore.DataService.Services.Infrastructure.Interfaces;

namespace WalletCore.DataService.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContexts();
            services.AddScoped<IExchangeRateMergeRepository, ExchangeRateMergeRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MergeExchangeRatesConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            // Register Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                var redisOptions = configuration.GetSection("Redis").Get<RedisOptions>();
                options.Configuration = redisOptions.ConnectionString;
                options.InstanceName = redisOptions.InstanceName;
            });

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
