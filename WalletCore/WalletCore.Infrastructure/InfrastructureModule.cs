using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using WalletCore.Application.Interfaces;
using WalletCore.Infrastructure.HttpClientInfrastructure;

namespace WalletCore.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
        {
            services.AddTransient<ExternalHttpLoggingHandler>();
            services.AddECBHttpClient();
            services.AddWalletDataServiceHttpClient();
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Optional: retry on failure
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddScoped<ICommandPublisher, CommandPublisher>();
            return services;
        }
    }
}
