using Microsoft.Extensions.Logging;
using WalletCore.Application.Interfaces;
using WalletCore.Contracts.AdjustBalance;
using WalletCore.Contracts.CreateWallet;
using WalletCore.Contracts.DBModels;
using WalletCore.Contracts.EcbRateConverter;
using WalletCore.Contracts.GetBalance;
using WalletCore.Contracts.WalletStrategy;
using WalletCore.Contracts.Exceptions;
using WalletCore.Logging;

namespace WalletCore.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletBalanceStrategyFactory _strategyFactory;
        private readonly IEcbRateConverter _rateConverter;
        private readonly IWalletDataServiceHttpClient _walletDataServiceHttpClient;
        private readonly WalletServiceLogger _log;

        public WalletService(
            IWalletBalanceStrategyFactory strategyFactory,
            IEcbRateConverter rateConverter,
            IWalletDataServiceHttpClient walletDataServiceHttpClient,
            WalletServiceLogger log)
        {
            _strategyFactory = strategyFactory;
            _rateConverter = rateConverter;
            _walletDataServiceHttpClient = walletDataServiceHttpClient;
            _log = log;
        }

        public async Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request)
        {
            _log.LogCreatingWallet(request);
            try
            {
                var response = await _walletDataServiceHttpClient.CreateWalletAsync(request);

                _log.LogWalletCreated(new Wallet { Id = response.WalletId, Balance = 0, Currency = request.Currency });

                return response;
            }
            catch (Exception ex)
            {
                _log.LogWalletCreationFailed(new Wallet { Currency = request.Currency }, ex);
                throw;
            }
        }

        public async Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            _log.LogFetchingBalance(request);

            var wallet = await _walletDataServiceHttpClient.GetWalletByIdAsync(request.WalletId)
                ?? throw new WalletException.WalletNotFoundException(request.WalletId);

            string targetCurrency = string.IsNullOrWhiteSpace(request.ConvertToCurrency)
                ? wallet.Currency
                : request.ConvertToCurrency;

            if (string.Equals(wallet.Currency, targetCurrency, StringComparison.OrdinalIgnoreCase))
            {
                _log.LogBalanceWithoutConversion(wallet);

                return new GetBalanceResponse
                {
                    WalletId = wallet.Id,
                    Balance = wallet.Balance,
                    Currency = wallet.Currency
                };
            }

            _log.LogBalanceConversion(wallet, targetCurrency);

            var conversion = await _rateConverter.ConvertAsync(new CurrencyConversionRequest
            {
                Amount = wallet.Balance,
                FromCurrency = wallet.Currency,
                ToCurrency = targetCurrency
            });

            var convertedBalance = Math.Round(
                conversion.ConvertedAmount,
                2,
                MidpointRounding.AwayFromZero);

            _log.LogBalanceWithConversion(wallet, convertedBalance, targetCurrency);

            return new GetBalanceResponse
            {
                WalletId = wallet.Id,
                Balance = convertedBalance,
                Currency = targetCurrency
            };
        }


        public async Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequest request)
        {
            _log.LogAdjustBalanceRequest(request);

            var wallet = await _walletDataServiceHttpClient.GetWalletByIdAsync(request.WalletId)
                ?? throw new WalletException.WalletNotFoundException(request.WalletId);

            var oldBalance = wallet.Balance;

            _log.LogWalletLoadedForAdjustment(wallet);

            var strategy = _strategyFactory.Create(request.AdjustmentStrategy);
            _log.LogApplyingStrategy(request);

            var conversion = await _rateConverter.ConvertAsync(new CurrencyConversionRequest
            {
                Amount = request.Amount,
                FromCurrency = request.AmountCurrency,
                ToCurrency = wallet.Currency
            });

            var walletAdjustmentOperation = new WalletBalanceStrategyOperation
            {
                CurrentBalance = wallet.Balance,
                Amount = conversion.ConvertedAmount,
                Currency = wallet.Currency
            };

            var walletAdjustmentResult = strategy.Apply(walletAdjustmentOperation);
            _log.LogBalanceCalculated(wallet, oldBalance, conversion.ConvertedAmount, walletAdjustmentResult.NewBalance, request.AdjustmentStrategy);

            var walletDataRequest = new AdjustBalanceRequestDto
            {
                Wallet = wallet,
                NewBalance = walletAdjustmentResult.NewBalance
            };

            var result = await _walletDataServiceHttpClient.AdjustBalanceAsync(walletDataRequest);

            var roundedOldBalance = Math.Round(oldBalance, 2, MidpointRounding.AwayFromZero);
            var roundedNewBalance = Math.Round(result.NewBalance, 2, MidpointRounding.AwayFromZero);
            var roundedAppliedAmount = Math.Round(conversion.ConvertedAmount, 2, MidpointRounding.AwayFromZero);

            _log.LogBalanceAdjusted(result.WalletId, roundedOldBalance, roundedNewBalance, roundedAppliedAmount, result.WalletCurrency, request.AdjustmentStrategy);

            return new AdjustBalanceResponse
            {
                WalletId = result.WalletId,
                OldBalance = roundedOldBalance,
                NewBalance = roundedNewBalance,
                AppliedAmount = roundedAppliedAmount,
                WalletCurrency = result.WalletCurrency,
                IsSuccessful = true,
                AdjustmentStrategy = request.AdjustmentStrategy,
                Message = request.AdjustmentStrategy.ToMessage()
            };
        }
    }
}