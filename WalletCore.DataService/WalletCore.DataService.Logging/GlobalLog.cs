namespace WalletCore.DataService.Logging
{
    public class GlobalLog
    {
        public string TransactionId { get; init; }
        public string RequestUUID { get; init; }
        public string Direction { get; init; }
        public string Service { get; init; }
        public object? Payload { get; init; }
        public DateTime Timestamp { get; init; }
        public string Level { get; init; }
        public string Endpoint { get; init; }
        public string Environment { get; init; }
        public string Message { get; init; }
        public string Exception { get; init; }
    }
}
