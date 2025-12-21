using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Domain.Exceptions;

namespace WalletCore.Application.Strategies
{
    public class WalletBalanceStrategyFactory : IWalletBalanceStrategyFactory
    {
        public IWalletBalanceStrategy Create(WalletStrategyOperation adjustmentStrategy)
        {
            return adjustmentStrategy switch
            {
                WalletStrategyOperation.AddFunds => new AddFundsStrategy(),
                WalletStrategyOperation.SubtractFunds => new SubtractFundsStrategy(),
                WalletStrategyOperation.ForceSubtractFunds => new ForceSubtractFundsStrategy(),
                _ => throw new WalletException.StrategyNotFoundException(adjustmentStrategy)
            };
        }
    }
}
