using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.GetDailyRates;

namespace WalletCore.Application.Services
{
    public class EcbService : IEcbService
    {
        private readonly IECBClient _ecbClient;
        private readonly ILogger<EcbService> _logger;

        // Constructor injection of typed client - clean and type-safe!
        public EcbService(IECBClient ecbClient, ILogger<EcbService> logger)
        {
            _ecbClient = ecbClient;
            _logger = logger;
        }

        public async Task<GesmesEnvelope> GetDailyRatesAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching latest exchange rates from ECB");

            var response = await _ecbClient.GetXmlAsync<GesmesEnvelope>("/stats/eurofxref/eurofxref-daily.xml", ct);

            _logger.LogInformation("Successfully fetched exchange rates");
            return response;
        }
    }
}
