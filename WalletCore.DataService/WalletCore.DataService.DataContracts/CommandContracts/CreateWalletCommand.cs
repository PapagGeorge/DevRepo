namespace WalletCore.DataService.DataContracts.CommandContracts
{
    /// <summary>
    /// Command to create a new wallet
    /// </summary>
    public record CreateWalletCommand(
        Guid WalletId,
        string Currency);
}
