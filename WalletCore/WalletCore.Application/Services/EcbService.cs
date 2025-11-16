using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.GetDailyRates;

namespace WalletCore.Application.Services
{
    public class EcbService : IEcbService
    {
        private readonly IGenericHttpClientFactory _clientFactory;
        private readonly ECBClientConfig _config;

        public EcbService(IGenericHttpClientFactory clientFactory, IOptions<ECBClientConfig> options)
        {
            _clientFactory = clientFactory;
            _config = options.Value;
        }

        public async Task<GesmesEnvelope> GetDailyRatesAsync()
        {
            var client = _clientFactory.CreateClient(_config.ClientName);
            return await client.GetXmlAsync<GesmesEnvelope>("/stats/eurofxref/eurofxref-daily.xml");
        }
    }
}
