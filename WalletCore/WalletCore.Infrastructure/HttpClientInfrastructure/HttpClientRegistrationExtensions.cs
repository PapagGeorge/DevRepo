using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using WalletCore.Application.Interfaces;

namespace WalletCore.Infrastructure.HttpClientInfrastructure
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddECBHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IECBClient, ECBHttpClient>()
                .AddHttpMessageHandler<ExternalHttpLoggingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        // Optional: Add logging here
                        Console.WriteLine($"Retry {retryCount} after {timespan.TotalSeconds}s");
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, timespan) =>
                    {
                        // Optional: Add logging here
                        Console.WriteLine($"Circuit breaker opened for {timespan.TotalSeconds}s");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuit breaker reset");
                    });
        }
    }
}
