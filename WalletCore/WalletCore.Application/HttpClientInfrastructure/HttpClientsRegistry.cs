using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public class HttpClientsRegistry
    {
        private readonly ECBClientConfig _ecbConfig;

        public HttpClientsRegistry(IOptions<ECBClientConfig> ecbClientOptions)
        {
            _ecbConfig = ecbClientOptions.Value;
        }

        public HttpClientRegistration GetECBRegistration()
        {
            return new HttpClientRegistration
            {
                ClientName = _ecbConfig.ClientName,
                BaseAddress = _ecbConfig.BaseAddress
            };
        }
    }
}
