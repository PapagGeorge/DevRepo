using Microsoft.EntityFrameworkCore;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.DBModels;
using WalletCore.Domain.Exceptions;

namespace WalletCore.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _db;

        public WalletRepository(WalletDbContext db)
        {
            _db = db;
        }

        public async Task<Wallet> GetByIdAsync(Guid id)
        {
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.Id == id);

            if (wallet == null)
                throw new WalletException.WalletNotFoundException(id);

            return wallet;
        }

        public async Task AddAsync(Wallet wallet)
        {
            await _db.Wallets.AddAsync(wallet);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateBalanceAsync(Guid id, decimal newBalance)
        {
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.Id == id);
            if (wallet == null)
                throw new WalletException.WalletNotFoundException(id);

            wallet.Balance = newBalance;

            _db.Wallets.Update(wallet);
            await _db.SaveChangesAsync();
        }
    }
}
