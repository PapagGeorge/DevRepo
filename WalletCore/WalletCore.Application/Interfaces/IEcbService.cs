using WalletCore.Domain.DBModels;

namespace WalletCore.Application.Interfaces
{
    public interface IEcbService
    {
        Task<List<ExchangeRate>> GetDailyRatesAsync(CancellationToken ct = default);
    }
}
