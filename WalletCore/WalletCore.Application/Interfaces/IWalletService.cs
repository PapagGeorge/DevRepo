using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Contrtacts.CreateWallet;
using WalletCore.Contrtacts.GetBalance;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletService
    {
        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request);

        Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);

        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequest request);
    }
}
