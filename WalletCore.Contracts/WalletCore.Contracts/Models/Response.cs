namespace WalletCore.Contracts.Models
{
    public class Response<T>
    {
        public T Payload { get; set; }
        public ResponseMessage Exception { get; set; }
    }
}
