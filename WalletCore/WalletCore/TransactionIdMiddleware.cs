using Serilog.Context;
using System.Text.Json;

namespace WalletCore
{
    public class TransactionIdMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            string transactionId = null;

            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        var json = JsonDocument.Parse(body);
                        if (json.RootElement.TryGetProperty("transactionId", out var idProp))
                        {
                            transactionId = idProp.GetString();
                        }
                    }
                    catch
                    {
                        // ignore parsing errors for non-JSON requests
                    }
                }
            }

            if (!string.IsNullOrEmpty(transactionId))
            {
                using (LogContext.PushProperty("TransactionId", transactionId))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }   
}
