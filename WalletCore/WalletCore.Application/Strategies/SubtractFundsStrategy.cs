using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models.WalletStrategy;

namespace WalletCore.Application.Strategies
{
    public class SubtractFundsStrategy : IWalletBalanceStrategy
    {
        public WalletBalanceStrategyResult Apply(WalletBalanceStrategyOperation operation)
        {
            if (operation.CurrentBalance < operation.Amount)
                throw new InvalidOperationException("Insufficient funds.");

            return new WalletBalanceStrategyResult()
            {
                NewBalance = operation.CurrentBalance - operation.Amount,
                IsSuccessful = true
            };
        }
    }
}
