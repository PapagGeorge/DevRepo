using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletCore.Application.HttpClientInfrastructure;
using WalletCore.Application.Interfaces;
using WalletCore.Application.Services;

namespace WalletCore.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<GenericHttpClientBuilder> configure)
        {
            var builder = new GenericHttpClientBuilder(services);
            configure(builder);

            services.AddSingleton<IGenericHttpClientFactory, GenericHttpClientFactory>();
            // Register the raw HTTP service
            services.AddScoped<EcbService>();

            // Register IEcbService as the cached decorator

            return services;
        }
    }
}
