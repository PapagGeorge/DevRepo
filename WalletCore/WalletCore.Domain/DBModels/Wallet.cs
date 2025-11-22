namespace WalletCore.Domain.DBModels
{
    public class Wallet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
