using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.GetDailyRates;

namespace WalletCore.Application.Services
{
    public class EcbService
    {
        private readonly IGenericHttpClientFactory _clientFactory;

        public EcbService(IGenericHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<GesmesEnvelope> GetDailyRatesAsync()
        {
            var client = _clientFactory.CreateClient("ECBApi");
            return await client.GetXmlAsync<GesmesEnvelope>("/stats/eurofxref/eurofxref-daily.xml");
        }
    }
}
