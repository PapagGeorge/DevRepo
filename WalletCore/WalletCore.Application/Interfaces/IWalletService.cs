using WalletCore.Domain.Models.AdjustBalance;
using WalletCore.Domain.Models.CreateWallet;
using WalletCore.Domain.Models.GetBalance;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletService
    {
        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request);

        Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request);

        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequest request);
    }
}
