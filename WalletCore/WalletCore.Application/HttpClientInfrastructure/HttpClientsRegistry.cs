using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public static class HttpClientsRegistry
    {
        public static HttpClientRegistration ECBClientRegistry(IOptions<ECBClientConfig> options)
        {
            var config = options.Value;

            return new HttpClientRegistration
            {
                ClientName = config.ClientName,
                BaseAddress = config.BaseAddress
            };
        }
    }
}
