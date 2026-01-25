using WalletCore.Contracts.AdjustBalance;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletBalanceStrategyFactory
    {
        IWalletBalanceStrategy Create(WalletStrategyOperation adjustmentStrategy);
    }
}
