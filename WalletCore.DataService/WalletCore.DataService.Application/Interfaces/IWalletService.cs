using WalletCore.Contrtacts.CreateWallet;
using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Contrtacts.DBModels;

namespace WalletCore.DataService.Application.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet?> GetByIdAsync(Guid id);
        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request);
        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequestDto request);
    }
}
