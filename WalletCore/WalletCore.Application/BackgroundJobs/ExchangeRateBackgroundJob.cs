using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;
using WalletCore.Domain.Models.GetDailyRates;

namespace WalletCore.Application.BackgroundJobs
{
    public class ExchangeRateBackgroundJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExchangeRateBackgroundJob> _logger;
        private const string CacheKey = "latest_exchange_rates";

        public ExchangeRateBackgroundJob(IServiceProvider serviceProvider, ILogger<ExchangeRateBackgroundJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Exchange rate background job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var ecbService = scope.ServiceProvider.GetRequiredService<IEcbService>();
                    var mergeRepo = scope.ServiceProvider.GetRequiredService<IExchangeRateMergeRepository>();
                    var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();

                    await RunJobAsync(ecbService, mergeRepo, cache, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while running exchange rate job.");
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
            _logger.LogInformation("Fetching ECB daily rates...");

            var xml = await ecbService.GetDailyRatesAsync();
            var rates = ParseRates(xml);

            await mergeRepo.MergeRatesAsync(rates, ct);

            await cacheService.UpdateExchangeRatesAsync(rates, ct);

            _logger.LogInformation("Exchange rates updated and cached.");
        }

        private static List<ExchangeRate> ParseRates(GesmesEnvelope xml)
        {
            var timeCube = xml.Cube.TimeCubes.First(); // Daily rates → 1 element
            var date = DateOnly.Parse(timeCube.Time);

            return timeCube.Rates.Select(r =>
                new ExchangeRate
                {
                    Date = date,
                    CurrencyCode = r.Currency,
                    Rate = r.Rate,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();
        }
    }
}
