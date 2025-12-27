using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.DBModels;
using WalletCore.Logging;

namespace WalletCore.Application.Services
{
    public class CachedEcbService : IEcbService
    {
        private readonly IEcbService _inner;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachedEcbService> _logger;
        private const string CacheKey = "latest_exchange_rates";

        public CachedEcbService(IEcbService inner, IDistributedCache cache, ILogger<CachedEcbService> logger)
        {
            _inner = inner;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<ExchangeRate>> GetDailyRatesAsync(CancellationToken ct = default)
        {
            try
            {
                var json = await _cache.GetStringAsync(CacheKey);
                if (!string.IsNullOrEmpty(json))
                {
                    _logger.LogInfoExt("Cache hit: returning ECB rates from Redis.");
                    var cachedRates = JsonSerializer.Deserialize<List<ExchangeRate>>(json);
                    _logger.LogInfoExt("ECB rates retrieved from cache", b => b.WithPayload(cachedRates));
                    return cachedRates;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfoExt("Cache miss: fetching ECB rates from API.");
            }

            var exchanggeRates = await _inner.GetDailyRatesAsync(); // Call API
            var serialized = JsonSerializer.Serialize(exchanggeRates);

            try
            {
                await _cache.SetStringAsync(CacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                }, ct);

                _logger.LogInfoExt("ECB rates cached successfully", b =>
                    b.WithPayload(exchanggeRates));
            }
            catch (Exception ex)
            {
                _logger.LogWarningExt("Failed to write ECB rates to cache.", ex);
            }

            return exchanggeRates;
        }
    }
}
