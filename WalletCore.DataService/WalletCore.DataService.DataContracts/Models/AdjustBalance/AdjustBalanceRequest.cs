namespace WalletCore.DataService.DataContracts.Models.AdjustBalance
{
    public class AdjustBalanceRequest
    {
        public Wallet Wallet { get; set; }
        public decimal NewBalance { get; set; }
    }
}
