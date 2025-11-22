namespace WalletCore.Domain.Models
{
    public class Response<T>
    {
        public T Payload { get; set; }
        public ResponseMessage Exception { get; set; }
    }
}
