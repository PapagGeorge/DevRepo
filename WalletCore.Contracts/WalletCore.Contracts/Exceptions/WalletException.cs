using System;
using WalletCore.Contrtacts.AdjustBalance;

namespace WalletCore.Domain.Exceptions
{
    public static class WalletException
    {
        // Base exception for all business domain errors
        public abstract class BusinessException : Exception
        {
            protected BusinessException(string message) : base(message) { }
        }

        public class WalletNotFoundException : BusinessException
        {
            public WalletNotFoundException(Guid walletId)
                : base($"Wallet with ID '{walletId}' was not found.") { }
        }

        public class InsufficientFundsException : BusinessException
        {
            public InsufficientFundsException(decimal balance, decimal attempt)
                : base($"Cannot subtract {attempt}. Current balance is {balance}.") { }
        }

        public class InvalidCurrencyException : BusinessException
        {
            public InvalidCurrencyException(string currency)
                : base($"Currency '{currency}' is not supported.") { }
        }

        public class StrategyNotFoundException : BusinessException
        {
            public StrategyNotFoundException(WalletStrategyOperation strategyName)
                : base($"Balance strategy '{strategyName}' does not exist.") { }
        }
    }
}
