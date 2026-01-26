using Microsoft.Extensions.Logging;
using System.Reflection;

namespace WalletCore.Logging
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs an informational message with optional structured enrichment.
        /// </summary>
        public static void LogInformation(
            this ILogger logger,
            string message,
            Action<GlobalLogBuilder> enrich)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevel.Information.ToString())
                .WithMessage(message);

            enrich.Invoke(builder);

            var log = builder.Build();
            logger.LogInformation("{@GlobalLog}", log);
        }

        /// <summary>
        /// Logs a warning message with optional exception and structured enrichment.
        /// </summary>
        public static void LogWarning(
            this ILogger logger,
            string message,
            Exception? exception,
            Action<GlobalLogBuilder> enrich)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevel.Warning.ToString())
                .WithMessage(message)
                .WithException(exception);

            enrich.Invoke(builder);

            var log = builder.Build();
            logger.LogWarning(exception, "{@GlobalLog}", log);
        }

        /// <summary>
        /// Logs a warning message with structured enrichment.
        /// </summary>
        public static void LogWarning(
            this ILogger logger,
            string message,
            Action<GlobalLogBuilder> enrich)
        {
            LogWarning(logger, message, null, enrich);
        }

        /// <summary>
        /// Logs an error message with optional exception and structured enrichment.
        /// </summary>
        public static void LogError(
            this ILogger logger,
            string message,
            Exception? exception,
            Action<GlobalLogBuilder> enrich)
        {
            var builder = CreateBaseBuilder()
                .WithLevel(LogLevel.Error.ToString())
                .WithMessage(message)
                .WithException(exception);

            enrich.Invoke(builder);

            var log = builder.Build();
            logger.LogError(exception, "{@GlobalLog}", log);
        }

        /// <summary>
        /// Logs an error message with structured enrichment.
        /// </summary>
        public static void LogError(
            this ILogger logger,
            string message,
            Action<GlobalLogBuilder> enrich)
        {
            LogError(logger, message, null, enrich);
        }

        private static GlobalLogBuilder CreateBaseBuilder()
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

    public enum LogDirection { Inbound, Outbound }
}
