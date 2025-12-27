using Microsoft.AspNetCore.Http;

namespace WalletCore.Logging
{
    public static class HttpAccessor
    {
        public static IHttpContextAccessor Accessor { get; private set; }

        public static void Configure(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }
    }
}
