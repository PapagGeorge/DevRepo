using MassTransit;
using Microsoft.Extensions.Logging;
using WalletCore.DataService.DataContracts;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.DataService.Infrastructure.Interfaces;

namespace WalletCore.DataService.Infrastructure.Consumers
{
    public class MergeExchangeRatesConsumer
        : IConsumer<MergeExchangeRatesCommand>
    {
        private readonly IExchangeRateMergeRepository _repository;
        private readonly ILogger<MergeExchangeRatesConsumer> _logger;

        public MergeExchangeRatesConsumer(
            IExchangeRateMergeRepository repository,
            ILogger<MergeExchangeRatesConsumer> logger)
        {
            _repository = repository;
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

            await _repository.MergeRatesAsync(
                rates,
                context.CancellationToken);

            _logger.LogInformation(
                "Exchange rates merged successfully");
        }
    }
}
