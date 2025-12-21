using WalletCore.Contrtacts.DBModels;

namespace WalletCore.DataService.Infrastructure.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByIdAsync(Guid id);
        Task AddAsync(Wallet wallet);
        Task UpdateBalanceAsync(Wallet wallet, decimal newBalance);
    }
}
