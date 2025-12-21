using System;
using WalletCore.Contrtacts.DBModels;

namespace WalletCore.Contrtacts.CommandContracts
{
    /// <summary>
    /// Command to create a new wallet
    /// </summary>
    public record CreateWalletCommand(
        Wallet wallet);
}
