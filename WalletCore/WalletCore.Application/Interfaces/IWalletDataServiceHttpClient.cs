using WalletCore.Domain.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface IWalletDataServiceHttpClient
    {
        Task<Wallet> GetWalletById(Guid id, CancellationToken ct = default);
    }
}
