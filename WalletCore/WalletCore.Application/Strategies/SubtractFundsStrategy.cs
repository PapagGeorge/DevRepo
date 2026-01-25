using WalletCore.Application.Interfaces;
using WalletCore.Contracts.WalletStrategy;
using WalletCore.Contracts.Exceptions;

namespace WalletCore.Application.Strategies
{
    public class SubtractFundsStrategy : IWalletBalanceStrategy
    {
        public WalletBalanceStrategyResult Apply(WalletBalanceStrategyOperation operation)
        {
            if (operation.CurrentBalance < operation.Amount)
                throw new WalletException.InsufficientFundsException(operation.CurrentBalance, operation.Amount);

            return new WalletBalanceStrategyResult
            {
                NewBalance = operation.CurrentBalance - operation.Amount,
                IsSuccessful = true
            };
        }
    }
}
