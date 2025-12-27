using Microsoft.Extensions.Logging;
using System.Reflection;

namespace WalletCore.Logging
{
    public static class LoggerExtensions
    {
        public static void LogErrorExt(
            this ILogger logger,
            string message,
            Exception? exception = null,
            Action<GlobalLogBuilder>? enrich = null)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevelExt.Error.ToString())
                .WithMessage(message)
                .WithException(exception);

            enrich?.Invoke(builder);

            var log = builder.Build();
            logger.LogError(exception, "{@GlobalLog}", log);
        }

        public static void LogInfoExt(
            this ILogger logger,
            string message,
            Action<GlobalLogBuilder>? enrich = null)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevelExt.Information.ToString())
                .WithMessage(message);

            enrich?.Invoke(builder);

            var log = builder.Build();

            logger.LogInformation("{@GlobalLog}", log);
        }

        public static void LogWarningExt(
            this ILogger logger,
            string message,
            Exception? exception = null,
            Action<GlobalLogBuilder>? enrich = null)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevelExt.Warning.ToString())
                .WithMessage(message)
                .WithException(exception);

            enrich?.Invoke(builder);

            var log = builder.Build();

            logger.LogWarning("{@GlobalLog}", log);
        }

        public static void LogRequestExt(
            this ILogger logger,
            string message,
            object? payload,
            Action<GlobalLogBuilder>? enrich = null)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevelExt.Information.ToString())
                .WithDirection(LogDirection.Inbound.ToString())
                .WithMessage(message)
                .WithPayload(payload);

            enrich?.Invoke(builder);

            var log = builder.Build();

            logger.LogInformation("{@GlobalLog}", log);
        }

        public static void LogResponseExt(
            this ILogger logger,
            string message,
            object? payload,
            Action<GlobalLogBuilder>? enrich = null)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevelExt.Information.ToString())
                .WithDirection(LogDirection.Outbound.ToString())
                .WithMessage(message)
                .WithPayload(payload);

            enrich?.Invoke(builder);

            var log = builder.Build();

            logger.LogInformation("{@GlobalLog}", log);
        }

        public static GlobalLogBuilder CreateBaseBuilder()
        {
            var http = HttpAccessor.Accessor?.HttpContext;
            var transactionId = http?.Items["TransactionId"]?.ToString();
            var endpoint = http?.Request?.Path.Value;
            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            return new GlobalLogBuilder()
                .WithService(assemblyName)
                .WithEnvironment(environment)
                .WithTransactionId(transactionId)
                .WithEndpoint(endpoint);
        }
    }

    public enum LogLevelExt { Information, Warning, Error }
    public enum LogDirection { Inbound, Outbound }
}
