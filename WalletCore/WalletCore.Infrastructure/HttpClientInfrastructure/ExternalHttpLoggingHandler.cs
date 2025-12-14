using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using WalletCore.Logging;

namespace WalletCore.Infrastructure
{
    public sealed class ExternalHttpLoggingHandler : DelegatingHandler
    {
        private readonly ILogger<ExternalHttpLoggingHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExternalHttpLoggingHandler(
            ILogger<ExternalHttpLoggingHandler> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var transactionId = _httpContextAccessor.HttpContext?.Items["TransactionId"]?.ToString();

            // Prepare common info
            var serviceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownService";
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var stopwatch = Stopwatch.StartNew();

            // Read request body safely
            string? requestBody = null;
            if (request.Content != null)
            {
                requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            // Log outgoing request
            var requestLog = new GlobalLogBuilder()
                .WithService(serviceName)
                .WithEnvironment(environment)
                .WithTransactionId(transactionId)
                .WithEndpoint($"{request.Method} {request.RequestUri}")
                .WithLevel(LogLevelExt.Information.ToString())
                .WithDirection(LogDirection.Outbound.ToString())
                .WithMessage("External HTTP request")
                .WithPayload(new { Headers = request.Headers, Body = requestBody })
                .Build();

            _logger.LogInformation("{@GlobalLog}", requestLog);

            HttpResponseMessage response;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                var errorLog = new GlobalLogBuilder()
                    .WithService(serviceName)
                    .WithEnvironment(environment)
                    .WithTransactionId(transactionId)
                    .WithEndpoint($"{request.Method} {request.RequestUri}")
                    .WithLevel(LogLevelExt.Error.ToString())
                    .WithDirection(LogDirection.Outbound.ToString())
                    .WithMessage("External HTTP call failed")
                    .WithPayload(new { DurationMs = stopwatch.ElapsedMilliseconds })
                    .WithException(ex)
                    .Build();

                _logger.LogError(ex, "{@GlobalLog}", errorLog);
                throw;
            }

            stopwatch.Stop();

            // Read response body safely
            string? responseBody = null;
            if (response.Content != null)
            {
                responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            }

            // Log response
            var responseLog = new GlobalLogBuilder()
                .WithService(serviceName)
                .WithEnvironment(environment)
                .WithTransactionId(transactionId)
                .WithEndpoint($"{request.Method} {request.RequestUri}")
                .WithLevel(LogLevelExt.Information.ToString())
                .WithDirection(LogDirection.Outbound.ToString())
                .WithMessage("External HTTP response")
                .WithPayload(new
                {
                    StatusCode = (int)response.StatusCode,
                    DurationMs = stopwatch.ElapsedMilliseconds,
                    Body = responseBody
                })
                .Build();

            _logger.LogInformation("{@GlobalLog}", responseLog);

            return response;
        }
    }
}
