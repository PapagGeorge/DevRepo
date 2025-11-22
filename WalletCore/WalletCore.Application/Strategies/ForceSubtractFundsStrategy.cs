using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.WalletStrategy;

namespace WalletCore.Application.Strategies
{
    public class ForceSubtractFundsStrategy : IWalletBalanceStrategy
    {
        public WalletBalanceStrategyResult Apply(WalletBalanceStrategyOperation operation)
            => new WalletBalanceStrategyResult
            {
                NewBalance = operation.CurrentBalance - operation.Amount,
                IsSuccessful = true
            };
    }
}
