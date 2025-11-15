using Microsoft.Extensions.DependencyInjection;

namespace WalletCore.Application.HttpClientInfrastructure
{
    public class GenericHttpClientBuilder
    {
        private readonly IServiceCollection _services;

        public GenericHttpClientBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public GenericHttpClientBuilder AddClient(HttpClientRegistration registration)
        {
            var builder = _services.AddHttpClient(registration.ClientName, client =>
            {
                client.BaseAddress = new Uri(registration.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
            }).SetHandlerLifetime(TimeSpan.FromMinutes(5));

            registration.Configure?.Invoke(builder);
            return this;
        }
    }
}
