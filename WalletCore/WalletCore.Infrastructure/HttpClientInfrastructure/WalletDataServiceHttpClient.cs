using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;
using WalletCore.Domain.Models.AdjustBalance;
using WalletCore.Domain.Models.CreateWallet;

namespace WalletCore.Infrastructure.HttpClientInfrastructure
{
    public class WalletDataServiceHttpClient : BaseHttpClient, IWalletDataServiceHttpClient
    {
        public WalletDataServiceHttpClient(
            HttpClient httpClient,
            ILogger<WalletDataServiceHttpClient> logger,
            IOptions<WalletDataServiceConfig> config)
            : base(httpClient, logger)
        {
            var walletDataConfig = config.Value;

            if (string.IsNullOrWhiteSpace(walletDataConfig.BaseAddress))
                throw new ArgumentException("WalletCore.DataService BaseAddress is required", nameof(config));

            HttpClient.BaseAddress = new Uri(walletDataConfig.BaseAddress);
            HttpClient.Timeout = TimeSpan.FromSeconds(walletDataConfig.TimeoutSeconds ?? 30);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", walletDataConfig.UserAgent ?? "WalletCore/1.0");
        }

        public Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequestDto request, CancellationToken ct = default)
        {
            var endpoint = "/wallet/balance/";
            return base.PostJsonAsync<AdjustBalanceRequestDto, AdjustBalanceResponse>(endpoint, request);
        }

        public Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request, CancellationToken ct = default)
        {
            var endpoint = "/wallet/";
            return base.PostJsonAsync<CreateWalletRequest, CreateWalletResponse>(endpoint, request);
        }

        public Task<Wallet> GetWalletByIdAsync(Guid id, CancellationToken ct = default)
        {
            var endpoint = $"/wallets/{id}";
            return base.GetJsonAsync<Wallet>(endpoint, ct);
        }
    }
}
