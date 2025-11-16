using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.GetDailyRates;

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

        public async Task<GesmesEnvelope> GetDailyRatesAsync()
        {
            try
            {
                var json = await _cache.GetStringAsync(CacheKey);
                if (!string.IsNullOrEmpty(json))
                {
                    _logger.LogInformation("Cache hit: returning ECB rates from Redis.");
                    return JsonSerializer.Deserialize<GesmesEnvelope>(json)!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read from cache.");
            }

            var xml = await _inner.GetDailyRatesAsync(); // Call API

            try
            {
                var serialized = JsonSerializer.Serialize(xml);
                await _cache.SetStringAsync(CacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write ECB rates to cache.");
            }

            return xml;
        }
    }
}
