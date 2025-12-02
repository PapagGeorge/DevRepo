using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WalletCore.Application.BackgroundJobs;
using WalletCore.Application.Configuration;
using WalletCore.Application.Interfaces;
using WalletCore.Application.Services;
using WalletCore.Application.Strategies;

namespace WalletCore.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // Register Strategies and Factories
            services.AddSingleton<IWalletBalanceStrategyFactory, WalletBalanceStrategyFactory>();

            // Register Core Services
            services.AddScoped<IEcbRateConverter, EcbRateConverter>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IWalletService, WalletService>();

            // Register ECB Service with Decorator Pattern
            services.AddScoped<EcbService>(); // Raw service
            services.AddScoped<IEcbService>(sp =>
            {
                var innerService = sp.GetRequiredService<EcbService>();
                var cache = sp.GetRequiredService<IDistributedCache>();
                var logger = sp.GetRequiredService<ILogger<CachedEcbService>>();
                return new CachedEcbService(innerService, cache, logger);
            });

            // Register Background Jobs
            services.AddHostedService<ExchangeRateBackgroundJob>();

            // Register Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                var redisOptions = configuration.GetSection("Redis").Get<RedisOptions>();
                options.Configuration = redisOptions.ConnectionString;
                options.InstanceName = redisOptions.InstanceName;
            });


            return services;
        }
    }
}
