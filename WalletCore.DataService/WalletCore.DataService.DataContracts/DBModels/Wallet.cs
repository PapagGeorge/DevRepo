namespace WalletCore.DataService.DataContracts
{
    public class Wallet
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
