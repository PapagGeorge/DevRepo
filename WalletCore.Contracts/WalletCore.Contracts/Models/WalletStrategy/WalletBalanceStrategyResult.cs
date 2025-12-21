namespace WalletCore.Contrtacts.WalletStrategy
{
    public class WalletBalanceStrategyResult
    {
        public decimal NewBalance { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
