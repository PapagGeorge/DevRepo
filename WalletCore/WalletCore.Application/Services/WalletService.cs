using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;
using WalletCore.Domain.Exceptions;
using WalletCore.Domain.Models.AdjustBalance;
using WalletCore.Domain.Models.CreateWallet;
using WalletCore.Domain.Models.EcbRateConverter;
using WalletCore.Domain.Models.GetBalance;
using WalletCore.Domain.Models.WalletStrategy;

namespace WalletCore.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletBalanceStrategyFactory _strategyFactory;
        private readonly IEcbRateConverter _rateConverter;
        private readonly ICommandPublisher _publisher;
        private readonly IWalletDataServiceHttpClient _walletDataServiceHttpClient;

        public WalletService(
            IWalletRepository walletRepository,
            IWalletBalanceStrategyFactory strategyFactory,
            IEcbRateConverter rateConverter,
            ICommandPublisher publisher,
            IWalletDataServiceHttpClient walletDataServiceHttpClient)
        {
            _walletRepository = walletRepository;
            _strategyFactory = strategyFactory;
            _rateConverter = rateConverter;
            _publisher = publisher;
            _walletDataServiceHttpClient = walletDataServiceHttpClient;
        }

        public async Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request)
        {
            var newWalletId = Guid.NewGuid();
            try
            {
                await _publisher.PublishCreateWalletAsync(newWalletId, request.Currency);

                return new CreateWalletResponse
                {
                    WalletId = newWalletId,
                    IsSuccessful = true,
                    Message = "Wallet created successfully."
                };
            }
            catch (Exception ex)
            {
                return new CreateWalletResponse
                {
                    WalletId = newWalletId,
                    IsSuccessful = false,
                    Message = $"Wallet creation failed: {ex.Message}"
                };
            }
        }

        public async Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var wallet = await _walletDataServiceHttpClient.GetWalletById(request.WalletId)
                ?? throw new WalletException.WalletNotFoundException(request.WalletId);

            string targetCurrency = string.IsNullOrWhiteSpace(request.ConvertToCurrency)
                ? wallet.Currency
                : request.ConvertToCurrency;

            if (string.Equals(wallet.Currency, targetCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return new GetBalanceResponse
                {
                    WalletId = wallet.Id,
                    Balance = wallet.Balance,
                    Currency = wallet.Currency
                };
            }

            var conversion = await _rateConverter.ConvertAsync(new CurrencyConversionRequest
            {
                Amount = wallet.Balance,
                FromCurrency = wallet.Currency,
                ToCurrency = targetCurrency
            });

            return new GetBalanceResponse
            {
                WalletId = wallet.Id,
                Balance = conversion.ConvertedAmount,
                Currency = targetCurrency
            };
        }

        public async Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequest request)
        {
            var wallet = await _walletDataServiceHttpClient.GetWalletById(request.WalletId)
                ?? throw new WalletException.WalletNotFoundException(request.WalletId);

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
            var oldBalance = wallet.Balance;

            await _publisher.PublishUpdateWalletBalanceAsync(wallet.Id, walletAdjustmentResult.NewBalance);

            return new AdjustBalanceResponse
            {
                WalletId = wallet.Id,
                OldBalance = oldBalance,
                NewBalance = walletAdjustmentResult.NewBalance,
                AppliedAmount = conversion.ConvertedAmount,
                WalletCurrency = wallet.Currency,
                IsSuccessful = true,
                AdjustmentStrategy = request.AdjustmentStrategy,
                Message = request.AdjustmentStrategy.ToMessage()
            };
        }
    }
}