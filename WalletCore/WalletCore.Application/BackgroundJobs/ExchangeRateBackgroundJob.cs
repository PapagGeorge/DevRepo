using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.Contrtacts.DBModels;
using WalletCore.Logging;

namespace WalletCore.Application.BackgroundJobs
{
    public class ExchangeRateBackgroundJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExchangeRateBackgroundJob> _logger;
        private readonly ICommandPublisher _publisher;

        public ExchangeRateBackgroundJob(
            IServiceProvider serviceProvider,
            ILogger<ExchangeRateBackgroundJob> logger,
            ICommandPublisher publisher)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfoExt("Exchange rate background job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var ecbService = scope.ServiceProvider
                        .GetRequiredKeyedService<IEcbService>("raw");

                    await RunJobAsync(ecbService, stoppingToken);
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
            CancellationToken ct)
        {
            _logger.LogInfoExt("Fetching ECB daily rates...");

            var exchangeRates = await ecbService.GetDailyRatesAsync(ct);

            var exchangeRatesDataRequest = new List<ExchangeRateDto>();

            exchangeRatesDataRequest = exchangeRates.Select(r => r.ToDataServiceRequest()).ToList();

            await _publisher.PublishMergeExchangeRatesAsync(exchangeRatesDataRequest);

            _logger.LogInfoExt("Exchange rates published successfully.");
        }
    }
}
