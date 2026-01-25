using WalletCore.Contracts.WalletStrategy;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletBalanceStrategy
    {
        WalletBalanceStrategyResult Apply(WalletBalanceStrategyOperation operation);
    }
}