using WalletCore.Contracts.AdjustBalance;
using WalletCore.Contracts.CreateWallet;
using WalletCore.Contracts.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletDataServiceHttpClient
    {
        Task<Wallet> GetWalletByIdAsync(Guid id, CancellationToken ct = default);

        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request, CancellationToken ct = default);

        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequestDto request, CancellationToken ct = default);
    }
}
