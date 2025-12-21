using WalletCore.Contrtacts.WalletStrategy;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletBalanceStrategy
    {
        WalletBalanceStrategyResult Apply(WalletBalanceStrategyOperation operation);
    }
}