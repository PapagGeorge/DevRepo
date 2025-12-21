using WalletCore.Domain.DBModels;
using WalletCore.Domain.Models.AdjustBalance;
using WalletCore.Domain.Models.CreateWallet;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletDataServiceHttpClient
    {
        Task<Wallet> GetWalletByIdAsync(Guid id, CancellationToken ct = default);

        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request, CancellationToken ct = default);

        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequestDto request, CancellationToken ct = default);
    }
}
