using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public class GenericHttpClientFactory : IGenericHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GenericHttpClient> _logger;

        public GenericHttpClientFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<GenericHttpClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public GenericHttpClient CreateClient(string name)
        {
            var httpClient = _httpClientFactory.CreateClient(name);
            return new GenericHttpClient(httpClient, _logger);
        }
    }
}
