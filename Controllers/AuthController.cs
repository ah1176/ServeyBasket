
using Survey_Basket.Contracts.Authentication;

namespace Survey_Basket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("")]
        public async Task<IActionResult> Login([FromBody]LoginRequest request , CancellationToken cancellation) 
        {
            var authResult = await _authService.GetTokenAsync(request.Email , request.Passowrd ,cancellation);

            return authResult.IsSuccess
            ? Ok(authResult.Value)
            : authResult.ToProblem(); 
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody]RefreshTokenRequest request, CancellationToken cancellation)
        {
            var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellation);

            return authResult is null ? BadRequest("Invalid token") : Ok(authResult);
        }

        [HttpPut("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefresh([FromBody]RefreshTokenRequest request, CancellationToken cancellation)
        {
            var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellation);

            return isRevoked  ? Ok() : BadRequest("operation failed") ;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellation)
        {
            var result = await _authService.RegisterAsync(request,cancellation);

            return result.IsSuccess
                ?Ok()
                :result.ToProblem();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request);

            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
        {
            var result = await _authService.ResndConfirmationEmailAsync(request);

            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await _authService.ForgetPasswordAsync(request.Email);

            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);

            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }
    }
}
