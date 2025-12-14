namespace WalletCore.Logging
{
    public class GlobalLogBuilder
    {
        private string _transactionId;
        private string _requestUuid;
        private string _direction;
        private string _service;
        private object? _payload;
        private DateTime _timestamp = DateTime.Now;
        private string _level;
        private string _endpoint;
        private string _environment;
        private string _message;
        private string _exception;

        public GlobalLogBuilder WithTransactionId(string transactionId)
        {
            _transactionId = transactionId;
            return this;
        }

        public GlobalLogBuilder WithRequestUUID(string requestUuid)
        {
            _requestUuid = requestUuid;
            return this;
        }

        public GlobalLogBuilder WithDirection(string direction)
        {
            _direction = direction;
            return this;
        }

        public GlobalLogBuilder WithService(string service)
        {
            _service = service;
            return this;
        }

        public GlobalLogBuilder WithPayload(object? payload)
        {
            _payload = payload;
            return this;
        }

        public GlobalLogBuilder WithLevel(string level)
        {
            _level = level;
            return this;
        }

        public GlobalLogBuilder WithEndpoint(string endpoint)
        {
            _endpoint = endpoint;
            return this;
        }

        public GlobalLogBuilder WithEnvironment(string environment)
        {
            _environment = environment;
            return this;
        }

        public GlobalLogBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public GlobalLogBuilder WithException(Exception? exception)
        {
            _exception = exception?.StackTrace;
            return this;
        }

        public GlobalLog Build()
        {
            return new GlobalLog
            {
                TransactionId = _transactionId,
                RequestUUID = _requestUuid,
                Direction = _direction,
                Service = _service,
                Payload = _payload,
                Timestamp = _timestamp,
                Level = _level,
                Endpoint = _endpoint,
                Environment = _environment,
                Message = _message,
                Exception = _exception
            };
        }
    }
}
