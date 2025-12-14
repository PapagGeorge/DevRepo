using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WalletCore.Application.Interfaces;
using WalletCore.Domain.Models;
using WalletCore.Domain.Models.AdjustBalance;
using WalletCore.Domain.Models.CreateWallet;
using WalletCore.Domain.Models.GetBalance;
using WalletCore.Logging;

namespace WalletCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly ILogger<WalletController> _logger;

        public WalletController(
            IWalletService walletService,
            ExceptionHandler exceptionHandler,
            ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _exceptionHandler = exceptionHandler;
            _logger = logger;
        }

        private async Task<Response<T>> HandleRequestAsync<T>(object request, Func<Task<T>> func)
        {
            var path = HttpContext.Request.Path.Value;

            _logger.LogRequestExt("Request to WalletCore", request);

            var response = await _exceptionHandler.HandleAsync(func);

            _logger.LogResponseExt("Response From WalletCore", response);

            return response;
        }

        [HttpPost("createWallet")]
        public async Task<Response<CreateWalletResponse>> CreateWallet(Request<CreateWalletRequest> request)
        {
            return await HandleRequestAsync(request, () => _walletService.CreateWalletAsync(request.Payload));
        }

        [HttpPost("getBalance")]
        public async Task<Response<GetBalanceResponse>> GetBalance(Request<GetBalanceRequest> request)
        {
            return await HandleRequestAsync(request, () => _walletService.GetBalanceAsync(request.Payload));
        }

        [HttpPost("adjustBalance")]
        public async Task<Response<AdjustBalanceResponse>> AdjustBalance(Request<AdjustBalanceRequest> request)
        {
            return await HandleRequestAsync(request, () => _walletService.AdjustBalanceAsync(request.Payload));
        }
    }
}
