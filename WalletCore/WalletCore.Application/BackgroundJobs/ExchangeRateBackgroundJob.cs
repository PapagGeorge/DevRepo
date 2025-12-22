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

                    var rates = await ecbService.GetDailyRatesAsync(stoppingToken);

                    var exchangeRatesDataRequest = new List<ExchangeRateDto>();

                    exchangeRatesDataRequest = rates.Select(r => r.ToDataServiceRequest()).ToList();

                    await publisher.PublishMergeExchangeRatesAsync(exchangeRatesDataRequest);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running exchange rate job");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
