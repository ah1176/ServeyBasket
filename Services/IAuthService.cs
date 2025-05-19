using Survey_Basket.Contracts.Authentication;

namespace Survey_Basket.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email , string password , CancellationToken cancellation);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token , string refreshToken , CancellationToken cancellation);
        Task<bool> RevokeRefreshTokenAsync(string token , string refreshToken , CancellationToken cancellation);

        Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellation);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

        Task<Result> ResndConfirmationEmailAsync(ResendConfirmationEmailRequest request);

        Task<Result> ForgetPasswordAsync(string email);

        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
