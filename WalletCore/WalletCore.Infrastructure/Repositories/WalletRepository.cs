using Microsoft.EntityFrameworkCore;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;

namespace WalletCore.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _db;

        public WalletRepository(WalletDbContext db)
        {
            _db = db;
        }

        public Task<Wallet?> GetByIdAsync(Guid id) =>
            _db.Wallets.FirstOrDefaultAsync(w => w.Id == id);

        public async Task AddAsync(Wallet wallet)
        {
            await _db.Wallets.AddAsync(wallet);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateBalanceAsync(Guid walletId, decimal newBalance)
        {
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.Id == walletId)
                         ?? throw new KeyNotFoundException("Wallet not found");

            wallet.Balance = newBalance;
            await _db.SaveChangesAsync();
        }
    }
}
