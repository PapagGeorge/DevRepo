using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private const string CacheKey = "latest_exchange_rates";

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task UpdateExchangeRatesAsync(IEnumerable<ExchangeRate> rates, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(rates);
            await _cache.SetStringAsync(CacheKey, json,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) },
                ct);
        }

        public async Task<List<ExchangeRate>> GetLatestExchangeRatesAsync(CancellationToken ct = default)
        {
            var json = await _cache.GetStringAsync(CacheKey, ct);
            if (!string.IsNullOrEmpty(json))
                return JsonSerializer.Deserialize<List<ExchangeRate>>(json) ?? new List<ExchangeRate>();

            return new List<ExchangeRate>();
        }
    }
}
