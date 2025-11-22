namespace WalletCore.Domain.Models.WalletStrategy
{
    public class WalletBalanceStrategyOperation
    {
        public decimal CurrentBalance { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
