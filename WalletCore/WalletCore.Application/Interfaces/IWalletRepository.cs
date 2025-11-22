using WalletCore.Domain.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByIdAsync(Guid id);
        Task AddAsync(Wallet wallet);
        Task UpdateBalanceAsync(Wallet wallet, decimal newBalance);
    }
}
