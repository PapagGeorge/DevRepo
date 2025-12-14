using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;
using WalletCore.Domain.Models.GetDailyRates;
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
                    return JsonSerializer.Deserialize<List<ExchangeRate>>(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarningExt("Failed to read from cache.", ex);
            }

            var exchanggeRates = await _inner.GetDailyRatesAsync(); // Call API
            var serialized = JsonSerializer.Serialize(exchanggeRates);

            try
            {
                await _cache.SetStringAsync(CacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarningExt("Failed to write ECB rates to cache.", ex);
            }

            return exchanggeRates;
        }
    }
}
