using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;
using WalletCore.Contracts.CommandContracts;
using WalletCore.Contracts.DBModels;
using WalletCore.Logging;

namespace WalletCore.Application.BackgroundJobs
{
    public class ExchangeRateBackgroundJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExchangeRateBackgroundJob> _logger;

        public ExchangeRateBackgroundJob(
            IServiceScopeFactory scopeFactory,
            ILogger<ExchangeRateBackgroundJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Exchange rate background job started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var ecbService = scope.ServiceProvider
                        .GetRequiredKeyedService<IEcbService>("raw");

                    var publisher = scope.ServiceProvider
                        .GetRequiredService<ICommandPublisher>();

                    _logger.LogInfoExt("Fetching daily exchange rates from ECB service");

                    var rates = await ecbService.GetDailyRatesAsync(stoppingToken);

                    _logger.LogInfoExt("Daily exchange rates fetched", b =>
                        b.WithPayload(rates));

                    var exchangeRatesDataRequest = new List<ExchangeRateDto>();

                    _logger.LogInfoExt("Publishing exchange rates to data service", b =>
                        b.WithPayload(exchangeRatesDataRequest));

                    exchangeRatesDataRequest = rates.Select(r => r.ToDataServiceRequest()).ToList();

                    await publisher.PublishMergeExchangeRatesAsync(exchangeRatesDataRequest);

                    _logger.LogInfoExt("Exchange rates successfully published");
                }
                catch (Exception ex)
                {
                    _logger.LogErrorExt("Error running exchange rate job", ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
