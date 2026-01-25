using WalletCore.Contracts.DBModels;

namespace WalletCore.DataService.Application.Interfaces.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByIdAsync(Guid id);
        Task AddAsync(Wallet wallet);
        Task UpdateBalanceAsync(Wallet wallet, decimal newBalance);
    }
}
