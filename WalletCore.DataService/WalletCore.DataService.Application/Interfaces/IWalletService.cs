using WalletCore.Contracts.CreateWallet;
using WalletCore.Contracts.AdjustBalance;
using WalletCore.Contracts.DBModels;

namespace WalletCore.DataService.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request);
        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequestDto request);
    }
}
