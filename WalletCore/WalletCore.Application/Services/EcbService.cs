using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.GetDailyRates;

namespace WalletCore.Application.Services
{
    public class EcbService : IEcbService
    {
        private readonly IGenericHttpClientFactory _httpClientFactory;
        private readonly ECBClientConfig _config;

        public EcbService(IGenericHttpClientFactory httpClientFactory, IOptions<ECBClientConfig> options)
        {
            _httpClientFactory = httpClientFactory;
            _config = options.Value;
        }

        public async Task<GesmesEnvelope> GetDailyRatesAsync()
        {
            return await _httpClientFactory
                .CreateClient(_config.ClientName)
                .GetXmlAsync<GesmesEnvelope>("/stats/eurofxref/eurofxref-daily.xml");
        }
    }
}
