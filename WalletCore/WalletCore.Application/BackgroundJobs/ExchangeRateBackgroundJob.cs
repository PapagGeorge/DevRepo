using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;
using WalletCore.Application.Services;
using WalletCore.Logging;

namespace WalletCore.Application.BackgroundJobs
{
    public class ExchangeRateBackgroundJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExchangeRateBackgroundJob> _logger;

        public ExchangeRateBackgroundJob(IServiceProvider serviceProvider, ILogger<ExchangeRateBackgroundJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfoExt("Exchange rate background job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    // Resolve raw service directly (no cache)
                    var ecbService = scope.ServiceProvider.GetRequiredKeyedService<IEcbService>("raw");
                    var mergeRepo = scope.ServiceProvider.GetRequiredService<IExchangeRateMergeRepository>();
                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                    await RunJobAsync(ecbService, mergeRepo, cacheService, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogErrorExt("Error occurred while running exchange rate job.", ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task RunJobAsync(
            IEcbService ecbService,
            IExchangeRateMergeRepository mergeRepo,
            ICacheService cacheService,
            CancellationToken ct)
        {
            _logger.LogInfoExt("Fetching ECB daily rates...");

            // Fetch fresh exchange rates from ECB (no caching)
            var exchangeRates = await ecbService.GetDailyRatesAsync(ct);

            // Merge into DB
            await mergeRepo.MergeRatesAsync(exchangeRates, ct);

            // Update cache for app usage
            await cacheService.UpdateExchangeRatesAsync(exchangeRates, ct);

            _logger.LogInfoExt("Exchange rates updated and cached.");
        }
    }
}
