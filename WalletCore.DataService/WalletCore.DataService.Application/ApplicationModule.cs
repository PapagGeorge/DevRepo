using Microsoft.Extensions.DependencyInjection;
using WalletCore.DataService.Application.Interfaces;
using WalletCore.DataService.Application.Services;

namespace WalletCore.DataService.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IExchangeRateService, ExchangeRateService>();

            return services;
        }
    }
}
