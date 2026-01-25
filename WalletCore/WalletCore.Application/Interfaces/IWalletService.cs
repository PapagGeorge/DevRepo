using WalletCore.Contracts.AdjustBalance;
using WalletCore.Contracts.CreateWallet;
using WalletCore.Contracts.GetBalance;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletService
    {
        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request);

        Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);

        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequest request);
    }
}
