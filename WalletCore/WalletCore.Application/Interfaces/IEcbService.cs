using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface IEcbService
    {
        Task<List<ExchangeRate>> GetDailyRatesAsync(CancellationToken ct = default);
    }
}
