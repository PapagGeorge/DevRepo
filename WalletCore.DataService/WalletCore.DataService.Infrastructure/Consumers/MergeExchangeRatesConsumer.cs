using MassTransit;
using Microsoft.Extensions.Logging;
using WalletCore.Contrtacts.CommandContracts;
using WalletCore.Contrtacts.DBModels;
using WalletCore.DataService.Infrastructure.Interfaces;
using WalletCore.DataService.Services.Infrastructure.Interfaces;

namespace WalletCore.DataService.Infrastructure.Consumers
{
    public class MergeExchangeRatesConsumer
        : IConsumer<MergeExchangeRatesCommand>
    {
        private readonly IExchangeRateMergeRepository _repository;
        private readonly ILogger<MergeExchangeRatesConsumer> _logger;
        private readonly ICacheService _cacheService;

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

            await _cacheService.UpdateExchangeRatesAsync(
                rates,
                context.CancellationToken);

            _logger.LogInformation(
                "Redis cache updated successfully");
        }
    }
}
