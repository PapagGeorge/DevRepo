using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalletCore.DataService.Logging;
using WalletCore.DataService.Infrastructure;
using WalletCore.DataService.DataContracts;
using WalletCore.DataService.Infrastructure.Interfaces;

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
                _logger.LogWarningExt(
                    "Wallet not found",
                    enrich: b => b.WithPayload(new { WalletId = id }));

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

                _logger.LogInfoExt(
                    "Wallet created",
                    enrich: b => b.WithPayload(new
                    {
                        wallet.Id,
                        wallet.Balance
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogErrorExt(
                    "Failed to create wallet",
                    ex,
                    enrich: b => b.WithPayload(wallet));

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

                _logger.LogInfoExt(
                    "Wallet balance updated",
                    enrich: b => b.WithPayload(new
                    {
                        wallet.Id,
                        OldBalance = oldBalance,
                        NewBalance = newBalance
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogErrorExt(
                    "Failed to update wallet balance",
                    ex,
                    enrich: b => b.WithPayload(new
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
