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
        private readonly ILogger<WalletService> _logger;

        public WalletService(
            IWalletBalanceStrategyFactory strategyFactory,
            IEcbRateConverter rateConverter,
            IWalletDataServiceHttpClient walletDataServiceHttpClient,
            ILogger<WalletService> logger)
        {
            _strategyFactory = strategyFactory;
            _rateConverter = rateConverter;
            _walletDataServiceHttpClient = walletDataServiceHttpClient;
            _logger = logger;
        }

        public async Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request)
        {
            _logger.LogInformation("Creating wallet", b => b.WithPayload(request));

            try
            {
                var response = await _walletDataServiceHttpClient.CreateWalletAsync(request);

                _logger.LogInformation("Wallet created successfully", b => b.WithPayload(new
                {
                    WalletId = response.WalletId,
                    Currency = request.Currency
                }));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Wallet creation failed", ex, b => b.WithPayload(new
                {
                    Currency = request.Currency
                }));
                throw;
            }
        }

        public async Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            _logger.LogInformation("Fetching wallet balance", b => b.WithPayload(request));

            var wallet = await _walletDataServiceHttpClient.GetWalletByIdAsync(request.WalletId)
                ?? throw new WalletException.WalletNotFoundException(request.WalletId);

            string targetCurrency = string.IsNullOrWhiteSpace(request.ConvertToCurrency)
                ? wallet.Currency
                : request.ConvertToCurrency;

            if (string.Equals(wallet.Currency, targetCurrency, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Returning balance without conversion", b => b.WithPayload(new
                {
                    wallet.Id,
                    wallet.Balance,
                    wallet.Currency
                }));

                return new GetBalanceResponse
                {
                    WalletId = wallet.Id,
                    Balance = wallet.Balance,
                    Currency = wallet.Currency
                };
            }

            _logger.LogInformation("Converting wallet balance", b => b.WithPayload(new
            {
                wallet.Id,
                Amount = wallet.Balance,
                FromCurrency = wallet.Currency,
                ToCurrency = targetCurrency
            }));

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

            _logger.LogInformation("Balance converted successfully", b => b.WithPayload(new
            {
                wallet.Id,
                OriginalBalance = wallet.Balance,
                ConvertedBalance = convertedBalance,
                Currency = targetCurrency
            }));

            return new GetBalanceResponse
            {
                WalletId = wallet.Id,
                Balance = convertedBalance,
                Currency = targetCurrency
            };
        }

        public async Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequest request)
        {
            _logger.LogInformation("Adjusting wallet balance", b => b.WithPayload(request));

            var wallet = await _walletDataServiceHttpClient.GetWalletByIdAsync(request.WalletId)
                ?? throw new WalletException.WalletNotFoundException(request.WalletId);

            var oldBalance = wallet.Balance;

            var strategy = _strategyFactory.Create(request.AdjustmentStrategy);

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

            var walletDataRequest = new AdjustBalanceRequestDto
            {
                Wallet = wallet,
                NewBalance = walletAdjustmentResult.NewBalance
            };

            var result = await _walletDataServiceHttpClient.AdjustBalanceAsync(walletDataRequest);

            var roundedOldBalance = Math.Round(oldBalance, 2, MidpointRounding.AwayFromZero);
            var roundedNewBalance = Math.Round(result.NewBalance, 2, MidpointRounding.AwayFromZero);
            var roundedAppliedAmount = Math.Round(conversion.ConvertedAmount, 2, MidpointRounding.AwayFromZero);

            _logger.LogInformation("Wallet balance adjusted successfully", b => b.WithPayload(new
            {
                WalletId = result.WalletId,
                OldBalance = roundedOldBalance,
                NewBalance = roundedNewBalance,
                AppliedAmount = roundedAppliedAmount,
                WalletCurrency = result.WalletCurrency,
                Strategy = request.AdjustmentStrategy
            }));

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
