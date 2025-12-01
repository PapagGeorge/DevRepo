using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WalletCore.Application.Configuration;
using WalletCore.Application.Services;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddECBHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("ECBClient", (sp, client) =>
            {
                var ecbConfig = sp.GetRequiredService<IOptions<ECBClientConfig>>().Value;

                client.BaseAddress = new Uri(ecbConfig.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(ecbConfig.TimeoutSeconds ?? 30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", ecbConfig.UserAgent ?? "WalletCore/1.0");
            });

            return services;
        }
    }
}
