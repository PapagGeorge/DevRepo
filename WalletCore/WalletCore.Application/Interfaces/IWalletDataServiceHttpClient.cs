using WalletCore.Contrtacts.AdjustBalance;
using WalletCore.Contrtacts.CreateWallet;
using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletDataServiceHttpClient
    {
        Task<Wallet> GetWalletByIdAsync(Guid id, CancellationToken ct = default);

        Task<CreateWalletResponse> CreateWalletAsync(CreateWalletRequest request, CancellationToken ct = default);

        Task<AdjustBalanceResponse> AdjustBalanceAsync(AdjustBalanceRequestDto request, CancellationToken ct = default);
    }
}
