using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletCore.Domain.CommandContracts
{
    /// <summary>
    /// Command to merge a list of exchange rates into the database
    /// </summary>
    public record MergeExchangeRatesCommand(
        IReadOnlyCollection<ExchangeRateDto> Rates);

    /// <summary>
    /// Exchange rate DTO for MergeExchangeRatesCommand
    /// </summary>
    public record ExchangeRateDto(
        Guid Id,
        DateOnly Date,
        string CurrencyCode,
        decimal Rate,
        DateTime UpdatedAt);
}
