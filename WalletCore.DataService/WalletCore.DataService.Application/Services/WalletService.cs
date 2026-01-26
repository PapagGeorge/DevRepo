using Microsoft.Extensions.Logging;
using WalletCore.Contracts.AdjustBalance;
using WalletCore.Contracts.CreateWallet;
using WalletCore.Contracts.DBModels;
using WalletCore.DataService.Application.Interfaces;
using WalletCore.DataService.Application.Interfaces.Repositories;
using WalletCore.Logging;

namespace WalletCore.DataService.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletService> _logger;

        public WalletService(
            IWalletRepository walletRepository,
            ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<Wallet?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting wallet by id", b => b.WithPayload(new { WalletId = id }));

            return await _walletRepository.GetByIdAsync(id);
        }

        public async Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request)
        {
            _logger.LogInformation("Creating wallet", b => b.WithPayload(new { Currency = request.Currency }));

            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                Currency = request.Currency,
                Balance = 0m
            };

            await _walletRepository.AddAsync(wallet);

            _logger.LogInformation("Wallet created successfully", b => b.WithPayload(new { WalletId = wallet.Id }));

            return new CreateWalletResponse
            {
                WalletId = wallet.Id,
                IsSuccessful = true,
                Message = $"Wallet with {wallet.Id} created successfully"
            };
        }

        public async Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequestDto request)
        {
            _logger.LogInformation("Adjusting wallet balance", b => b.WithPayload(new
            {
                WalletId = request.Wallet.Id,
                NewBalance = request.NewBalance
            }));

            await _walletRepository.UpdateBalanceAsync(request.Wallet, request.NewBalance);

            _logger.LogInformation("Wallet balance adjusted successfully", b => b.WithPayload(new
            {
                WalletId = request.Wallet.Id,
                NewBalance = request.NewBalance
            }));

            return new AdjustBalanceResponse
            {
                WalletId = request.Wallet.Id,
                IsSuccessful = true,
                NewBalance = request.NewBalance,
                WalletCurrency = request.Wallet.Currency
            };
        }
    }
}
