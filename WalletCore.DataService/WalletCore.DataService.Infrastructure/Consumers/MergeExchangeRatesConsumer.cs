using MassTransit;
using Microsoft.Extensions.Logging;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.Contrtacts.DBModels;
using WalletCore.DataService.Application.Interfaces;

namespace WalletCore.DataService.Infrastructure.Consumers
{
    public class MergeExchangeRatesConsumer
        : IConsumer<MergeExchangeRatesCommand>
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<MergeExchangeRatesConsumer> _logger;

        public MergeExchangeRatesConsumer(
            IExchangeRateService exchangeRateService,
            ILogger<MergeExchangeRatesConsumer> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<MergeExchangeRatesCommand> context)
        {
            var ratesDto = context.Message.Rates;

            _logger.LogInformation(
                "Consuming MergeExchangeRatesCommand with {Count} rates",
                ratesDto.Count);

            var rates = ratesDto.Select(r => new ExchangeRate
            {
                Id = r.Id,
                Date = r.Date,
                CurrencyCode = r.CurrencyCode,
                Rate = r.Rate,
                UpdatedAt = r.UpdatedAt
            });

            await _exchangeRateService.MergeRatesAsync(
                rates,
                context.CancellationToken);

            _logger.LogInformation(
                "Exchange rates processing completed");
        }
    }
}
