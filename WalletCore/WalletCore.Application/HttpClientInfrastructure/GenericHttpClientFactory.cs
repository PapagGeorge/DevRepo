using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public class GenericHttpClientFactory : IGenericHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerFactory _loggerFactory;

        public GenericHttpClientFactory(
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory)
        {
            _httpClientFactory = httpClientFactory;
            _loggerFactory = loggerFactory;
        }

        public GenericHttpClient CreateClient(string name)
        {
            var httpClient = _httpClientFactory.CreateClient(name);
            var logger = _loggerFactory.CreateLogger<GenericHttpClient>();
            return new GenericHttpClient(httpClient, logger);
        }
    }
}
