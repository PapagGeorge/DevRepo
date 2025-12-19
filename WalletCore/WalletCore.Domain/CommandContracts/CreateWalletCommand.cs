namespace WalletCore.Domain.CommandContracts
{
    /// <summary>
    /// Command to create a new wallet
    /// </summary>
    public record CreateWalletCommand(
        Guid WalletId,
        string Currency);
}
