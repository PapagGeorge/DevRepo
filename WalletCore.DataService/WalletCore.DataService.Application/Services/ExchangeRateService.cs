using Microsoft.Extensions.Logging;
using WalletCore.Contracts.DBModels;
using WalletCore.DataService.Application.Interfaces;
using WalletCore.DataService.Application.Interfaces.Repositories;
using WalletCore.Logging;

namespace WalletCore.DataService.Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateMergeRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(
            IExchangeRateMergeRepository repository,
            ICacheService cacheService,
            ILogger<ExchangeRateService> logger)
        {
            _repository = repository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task MergeRatesAsync(
            IEnumerable<ExchangeRate> rates,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInfoExt(
                "Merging exchange rates",
                enrich: b => b.WithPayload(new { Count = rates.Count() }));

            await _repository.MergeRatesAsync(rates, cancellationToken);

            _logger.LogInfoExt("Exchange rates merged to database successfully");

            await _cacheService.UpdateExchangeRatesAsync(rates, cancellationToken);

            _logger.LogInfoExt("Exchange rates cache updated successfully");
        }
    }
}
