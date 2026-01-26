using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletCore.Logging;
using WalletCore.DataService.Application.Interfaces.Repositories;
using WalletCore.DataService.Infrastructure;
using WalletCore.Contracts.DBModels;

namespace WalletCore.DataService.Repositories.Repositorues
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _db;
        private readonly ILogger<WalletRepository> _logger;

        public WalletRepository(
            WalletDbContext db,
            ILogger<WalletRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Wallet> GetByIdAsync(Guid id)
        {
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.Id == id);

            if (wallet == null)
            {
                _logger.LogWarning("Wallet not found", b => b.WithPayload(new { WalletId = id }));
                throw new Exception($"Wallet with id: {id} not found");
            }

            return wallet;
        }

        public async Task AddAsync(Wallet wallet)
        {
            try
            {
                await _db.Wallets.AddAsync(wallet);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Wallet created", b => b.WithPayload(new
                {
                    wallet.Id,
                    wallet.Balance
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create wallet", ex, b => b.WithPayload(wallet));
                throw;
            }
        }

        public async Task UpdateBalanceAsync(Wallet wallet, decimal newBalance)
        {
            try
            {
                var oldBalance = wallet.Balance;

                wallet.Balance = newBalance;
                _db.Wallets.Update(wallet);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Wallet balance updated", b => b.WithPayload(new
                {
                    wallet.Id,
                    OldBalance = oldBalance,
                    NewBalance = newBalance
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update wallet balance", ex, b => b.WithPayload(new
                {
                    wallet.Id,
                    wallet.Balance,
                    NewBalance = newBalance
                }));
                throw;
            }
        }
    }
}
