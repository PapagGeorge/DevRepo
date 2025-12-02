using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletCore.Application.Configuration;
using WalletCore.Application.Interfaces;

namespace WalletCore.Infrastructure.HttpClientInfrastructure
{
    public class ECBHttpClient : BaseHttpClient, IECBClient
    {
        public ECBHttpClient(
            HttpClient httpClient,
            ILogger<ECBHttpClient> logger,
            IOptions<ECBClientConfig> config)
            : base(httpClient, logger)
        {
            var ecbConfig = config.Value;

            if (string.IsNullOrWhiteSpace(ecbConfig.BaseAddress))
                throw new ArgumentException("ECB BaseAddress is required", nameof(config));

            HttpClient.BaseAddress = new Uri(ecbConfig.BaseAddress);
            HttpClient.Timeout = TimeSpan.FromSeconds(ecbConfig.TimeoutSeconds ?? 30);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/xml");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", ecbConfig.UserAgent ?? "WalletCore/1.0");
        }

        public Task<TResponse> GetXmlAsync<TResponse>(string endpoint, CancellationToken ct = default)
            => base.GetXmlAsync<TResponse>(endpoint, ct);
    }
}
