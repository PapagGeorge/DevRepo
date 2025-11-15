using Microsoft.Extensions.DependencyInjection;

namespace WalletCore.Application.Configuration
{
    public class HttpClientRegistration
    {
        public string ClientName { get; set; }
        public string BaseAddress { get; set; }
        public Action<IHttpClientBuilder>? Configure = null;
    }
}
