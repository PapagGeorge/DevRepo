using WalletCore.Contracts.DBModels;

namespace WalletCore.Contracts.AdjustBalance
{
    public class AdjustBalanceRequestDto
    {
        public Wallet Wallet { get; set; }
        public decimal NewBalance { get; set; }
    }
}
