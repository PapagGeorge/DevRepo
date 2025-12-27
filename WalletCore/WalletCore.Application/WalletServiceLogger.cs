using Microsoft.Extensions.Logging;
using WalletCore.Application.Services;
using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Contrtacts.CreateWallet;
using WalletCore.Contrtacts.DBModels;
using WalletCore.Contrtacts.GetBalance;
using WalletCore.Logging;

namespace WalletCore.Application
{
    public sealed class WalletServiceLogger
    {
        private readonly ILogger<WalletService> _logger;

        public WalletServiceLogger(ILogger<WalletService> logger)
        {
            _logger = logger;
        }
        public void LogCreatingWallet(CreateWalletRequest request)
        {
            _logger.LogInfoExt("Creating wallet", b => b.WithPayload(request));
        }

        public void LogWalletCreated(Wallet wallet)
        {
            _logger.LogInfoExt(
                "Wallet created successfully",
                b => b.WithPayload(wallet));
        }

        public void LogWalletCreationFailed(Wallet wallet, Exception ex)
        {
            _logger.LogErrorExt(
                "Wallet creation failed",
                ex,
                b => b.WithPayload(wallet));
        }

        public void LogFetchingBalance(GetBalanceRequest request)
        {
            _logger.LogInfoExt(
                "Fetching wallet balance",
                b => b.WithPayload(request));
        }

        public void LogBalanceWithoutConversion(Wallet wallet)
        {
            _logger.LogInfoExt(
                "Wallet balance returned without currency conversion",
                b => b.WithPayload(new
                {
                    wallet.Id,
                    wallet.Balance,
                    wallet.Currency
                }));
        }

        public void LogBalanceConversion(Wallet wallet, string targetCurrency)
        {
            _logger.LogInfoExt(
                "Converting wallet balance to target currency",
                b => b.WithPayload(new
                {
                    wallet.Id,
                    Amount = wallet.Balance,
                    FromCurrency = wallet.Currency,
                    ToCurrency = targetCurrency
                }));
        }

        public void LogBalanceWithConversion(
            Wallet wallet,
            decimal convertedBalance,
            string targetCurrency)
        {
            _logger.LogInfoExt(
                "Wallet balance returned after currency conversion",
                b => b.WithPayload(new
                {
                    wallet.Id,
                    OriginalBalance = wallet.Balance,
                    ConvertedBalance = convertedBalance,
                    Currency = targetCurrency
                }));
        }

        public void LogAdjustBalanceRequest(AdjustBalanceRequest request)
        {
            _logger.LogInfoExt(
                "Adjust wallet balance request received",
                b => b.WithPayload(request));
        }

        public void LogWalletLoadedForAdjustment(Wallet wallet)
        {
            _logger.LogInfoExt(
                "Wallet loaded for balance adjustment",
                b => b.WithPayload(new
                {
                    wallet.Id,
                    wallet.Balance,
                    wallet.Currency
                }));
        }

        public void LogApplyingStrategy(AdjustBalanceRequest request)
        {
            _logger.LogInfoExt(
                "Applying balance adjustment strategy",
                b => b.WithPayload(new
                {
                    Strategy = request.AdjustmentStrategy,
                    Amount = request.Amount,
                    AmountCurrency = request.AmountCurrency
                }));
        }

        public void LogBalanceCalculated(
            Wallet wallet,
            decimal oldBalance,
            decimal appliedAmount,
            decimal newBalance,
            WalletStrategyOperation strategy)
        {
            _logger.LogInfoExt(
                "Wallet balance calculated after strategy application",
                b => b.WithPayload(new
                {
                    wallet.Id,
                    OldBalance = oldBalance,
                    AppliedAmount = appliedAmount,
                    NewBalance = newBalance,
                    Strategy = strategy
                }));
        }

        public void LogBalanceAdjusted(
            Guid walletId,
            decimal oldBalance,
            decimal newBalance,
            decimal appliedAmount,
            string currency,
            WalletStrategyOperation strategy)
        {
            _logger.LogInfoExt(
                "Wallet balance adjusted successfully",
                b => b.WithPayload(new
                {
                    WalletId = walletId,
                    OldBalance = oldBalance,
                    NewBalance = newBalance,
                    AppliedAmount = appliedAmount,
                    WalletCurrency = currency,
                    Strategy = strategy
                }));
        }
    }
}
