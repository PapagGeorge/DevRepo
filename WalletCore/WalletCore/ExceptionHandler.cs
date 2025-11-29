using WalletCore.Domain.Models;

namespace WalletCore
{
    public class ExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly Type[] _businessExceptions;

        public ExceptionHandler(ILogger<ExceptionHandler> logger, params Type[] businessExceptions)
        {
            _logger = logger;
            _businessExceptions = businessExceptions;
        }

        public async Task<Response<T>> HandleAsync<T>(Func<Task<T>> func)
        {
            try
            {
                var result = await func();
                return new Response<T> { Payload = result };
            }
            catch (Exception ex) when (_businessExceptions.Any(t => t.IsInstanceOfType(ex)))
            {
                _logger.LogWarning(ex, "Business exception");

                return new Response<T>
                {
                    Exception = new ResponseMessage
                    {
                        Category = "Business",
                        Description = ex.Message,
                        Code = ex.GetType().Name
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Technical exception");

                return new Response<T>
                {
                    Exception = new ResponseMessage
                    {
                        Category = "Technical",
                        Description = ex.Message,
                        Code = ex.GetType().Name
                    }
                };
            }
        }
    }
}
