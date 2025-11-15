using WalletCore.Domain.Models.GetDailyRates;

namespace WalletCore.Application.Interfaces
{
    public interface IEcbService
    {
        Task<GesmesEnvelope> GetDailyRatesAsync();
    }
}
