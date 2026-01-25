using System;
using WalletCore.Contracts.DBModels;

namespace WalletCore.Contracts.CommandContracts
{
    /// <summary>
    /// Command to create a new wallet
    /// </summary>
    public record CreateWalletCommand(
        Wallet wallet);
}
