using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;
using WalletCore.Contracts.DBModels;
using WalletCore.Contracts.GetDailyRates;
using WalletCore.Logging;

namespace WalletCore.Application.Services
{
    public class EcbService : IEcbService
    {
        private readonly IECBClient _ecbClient;
        private readonly ILogger<EcbService> _logger;

        public EcbService(IECBClient ecbClient, ILogger<EcbService> logger)
        {
            _ecbClient = ecbClient;
            _logger = logger;
        }

        public async Task<List<ExchangeRate>> GetDailyRatesAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching latest exchange rates from ECB");

            var xml = await _ecbClient.GetXmlAsync<GesmesEnvelope>("/stats/eurofxref/eurofxref-daily.xml", ct);
            var response = xml.ParseRates();

            _logger.LogInformation("Successfully fetched exchange rates");
            return response;
        }
    }
}
