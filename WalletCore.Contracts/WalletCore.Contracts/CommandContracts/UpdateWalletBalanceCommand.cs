namespace WalletCore.Contrtacts.CommandContracts
{
    /// <summary>
    /// Command to update the balance of an existing wallet.
    /// This is a fire-and-forget message sent to the DataService to adjust balances.
    /// </summary>
    public record UpdateWalletBalanceCommand(
        Guid WalletId,       // Wallet to update
        decimal NewBalance   // New balance to set
    );
}
