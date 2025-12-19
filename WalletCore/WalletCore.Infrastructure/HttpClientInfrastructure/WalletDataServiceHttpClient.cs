using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;

namespace WalletCore.Infrastructure.HttpClientInfrastructure
{
    public class WalletDataServiceHttpClient : BaseHttpClient, IWalletDataServiceHttpClient
    {
        public WalletDataServiceHttpClient(
            HttpClient httpClient,
            ILogger<WalletDataServiceHttpClient> logger,
            IOptions<ECBClientConfig> config)
            : base(httpClient, logger)
        {
            var ecbConfig = config.Value;

            if (string.IsNullOrWhiteSpace(ecbConfig.BaseAddress))
                throw new ArgumentException("WalletCore.DataService BaseAddress is required", nameof(config));
        }
        public Task<Wallet> GetWalletById(Guid id, CancellationToken ct = default)
        {
            var endpoint = $"/wallets/{id}";
            return base.GetJsonAsync<Wallet>(endpoint, ct);
        }
    }
}
