using Microsoft.AspNetCore.Http;

namespace WalletCore.DataService
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
            // Read TransactionId from header (set by WalletCore service)
            string? transactionId = null;

            if (context.Request.Headers.TryGetValue("X-Transaction-Id", out var headerValue))
            {
                transactionId = headerValue.ToString();
            }

            // Store in HttpContext.Items for logging access
            context.Items["TransactionId"] = transactionId;

            await _next(context);
        }
    }
}
