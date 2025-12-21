using WalletCore.Domain.DBModels;

namespace WalletCore.Domain.Models.AdjustBalance
{
    public class AdjustBalanceRequestDto
    {
        public Wallet Wallet { get; set; }
        public decimal NewBalance { get; set; }
    }
}
