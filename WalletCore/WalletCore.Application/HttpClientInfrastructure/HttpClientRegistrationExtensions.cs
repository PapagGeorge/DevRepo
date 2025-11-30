using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletCore.Application.Configuration;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddECBHttpClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var ecbConfig = configuration
                .GetSection("ECBConfig")
                .Get<ECBClientConfig>();

            if (ecbConfig == null)
                throw new InvalidOperationException("ECBConfig section is missing in configuration");

            services.AddHttpClient(ecbConfig.ClientName)
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(ecbConfig.BaseAddress);
                    client.Timeout = TimeSpan.FromSeconds(ecbConfig.TimeoutSeconds ?? 30);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", ecbConfig.UserAgent ?? "WalletCore/1.0");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(ecbConfig.HandlerLifetimeMinutes ?? 5));

            return services;
        }
    }
}
