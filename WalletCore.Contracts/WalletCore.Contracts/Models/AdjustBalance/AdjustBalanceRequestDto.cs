using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Contrtacts.AdjustBalance
{
    public class AdjustBalanceRequestDto
    {
        public Wallet Wallet { get; set; }
        public decimal NewBalance { get; set; }
    }
}
