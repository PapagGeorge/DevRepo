using System;
using System.Collections.Generic;

namespace WalletCore.Contrtacts.CommandContracts
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
