using WalletCore.Application.Interfaces;
using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Contrtacts.CreateWallet;
using WalletCore.Contrtacts.DBModels;
using WalletCore.Contrtacts.EcbRateConverter;
using WalletCore.Contrtacts.GetBalance;
using WalletCore.Contrtacts.WalletStrategy;
using WalletCore.Domain.Exceptions;

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
            var newWallet = new Wallet { Id = Guid.NewGuid(), Balance = 0, Currency = request.Currency };
            try
            {
                await _publisher.PublishCreateWalletAsync(newWallet);

                return new CreateWalletResponse
                {
                    WalletId = newWallet.Id,
                    IsSuccessful = true,
                    Message = "Wallet created successfully."
                };
            }
            catch (Exception ex)
            {
                return new CreateWalletResponse
                {
                    IsSuccessful = false,
                    Message = $"Wallet creation failed: {ex.Message}"
                };
            }
        }

        public async Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request)
        {
            var wallet = await _walletDataServiceHttpClient.GetWalletByIdAsync(request.WalletId)
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

            var walletDataRequest = new AdjustBalanceRequestDto {Wallet = wallet, NewBalance = walletAdjustmentResult.NewBalance };

            var result = await _walletDataServiceHttpClient.AdjustBalanceAsync(walletDataRequest);
            return new AdjustBalanceResponse
            {
                WalletId = result.WalletId,
                OldBalance = oldBalance,
                NewBalance = result.NewBalance,
                AppliedAmount = conversion.ConvertedAmount,
                WalletCurrency = result.WalletCurrency,
                IsSuccessful = true,
                AdjustmentStrategy = request.AdjustmentStrategy,
                Message = request.AdjustmentStrategy.ToMessage()
            };
        }
    }
}