namespace WalletCore.Application.Configuration
{
    public class WalletDataServiceConfig
    {
        public string ClientName { get; set; }
        public string BaseAddress { get; set; }
        public int? TimeoutSeconds { get; set; }
        public string UserAgent { get; set; }
        public int? HandlerLifetimeMinutes { get; set; }
    }
}
