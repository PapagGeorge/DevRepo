using WalletCore.Application.Interfaces;
using WalletCore.Domain.Exceptions;
using WalletCore.Domain.Models;

namespace WalletCore.Application.Strategies
{
    public class WalletBalanceStrategyFactory : IWalletBalanceStrategyFactory
    {
        public IWalletBalanceStrategy Create(string strategyName)
        {
            return strategyName.ToLower() switch
            {
                "addfunds" => new AddFundsStrategy(),
                "subtractfunds" => new SubtractFundsStrategy(),
                "forcesubtractfunds" => new ForceSubtractFundsStrategy(),
                _ => throw new WalletException.StrategyNotFoundException(strategyName)
            };
        }
    }
}
