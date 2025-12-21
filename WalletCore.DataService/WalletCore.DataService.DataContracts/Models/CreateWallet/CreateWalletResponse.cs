namespace WalletCore.DataService.DataContracts.Models.CreateWallet
{
    public class CreateWalletResponse
    {
        public Guid WalletId { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
