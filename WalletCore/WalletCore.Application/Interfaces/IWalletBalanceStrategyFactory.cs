using WalletCore.Contrtacts.AdjustBalance;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletBalanceStrategyFactory
    {
        IWalletBalanceStrategy Create(WalletStrategyOperation adjustmentStrategy);
    }
}
