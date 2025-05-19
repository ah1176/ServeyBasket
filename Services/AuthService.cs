using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Survey_Basket.Authentication;
using Survey_Basket.Contracts.Authentication;
using Survey_Basket.Errors;
using Survey_Basket.Helpers;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Survey_Basket.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager 
        , IJwtProvider jwtProvider
        ,SignInManager<ApplicationUser> signInManager
        ,ILogger<AuthService> logger
        ,IEmailSender emailSender 
        ,IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ApplicationDbContext _context = context;
        private readonly int _refreshTokenExpiryDays = 14;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellation)
        {
            //check user?
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            if(user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);



            //check password
            var result = await _signInManager.PasswordSignInAsync(user, password,false,true);

            if (result.Succeeded) 
            {
                //generate token
               var (userRoles, userPermissions) = await GetUserRolesAndPermission(user,cancellation);


                var (token, expiresIn) = _jwtProvider.GenerateToken(user,userRoles,userPermissions);

                var refreshToken = GenerateRefreshToken();

                var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

                user.RefreshTokens.Add(
                    new RefreshToken
                    {
                        Token = refreshToken,
                        ExpiresOn = refreshTokenExpirationDate
                    }
                    );

                await _userManager.UpdateAsync(user);

                var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpirationDate);

                return Result.Success(response);
            }

            var error = result.IsNotAllowed
                ? UserErrors.EmailNotConfirmed
                :result.IsLockedOut
                ?UserErrors.LockedOut
                :UserErrors.InvalidCredentials;

            return Result.Failure<AuthResponse>(error);
           
        }

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation)
        {
            var userId = _jwtProvider.ValidateToken(token);

            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);


            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);


            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);


            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedOut);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            
            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);


            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var (userRoles, userPermissions) = await GetUserRolesAndPermission(user, cancellation);

            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

            var newRefreshToken = GenerateRefreshToken();

            var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(
                new RefreshToken
                {
                    Token = newRefreshToken,
                    ExpiresOn = refreshTokenExpirationDate
                }
                );

            await _userManager.UpdateAsync(user);

            var response =  new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpirationDate);

            return Result.Success(response);
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation)
        {
            var userId = _jwtProvider.ValidateToken(token);

            if (userId is null)
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            
            if (user is null) 
                return false;

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive); 
            
            if (userRefreshToken is null) 
                return false;

            userRefreshToken.RevokedOn = DateTime.UtcNow;

           await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellation)
        {
            var emailIsExisit = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellation);

            if (emailIsExisit)
             return  Result.Failure(UserErrors.DuplicateEmail);

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,    
            };

            var result = await _userManager.CreateAsync(user,request.Password);

            if (result.Succeeded) 
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                _logger.LogInformation("Confirmation code : {code}",code);

                await SendEmail(user, code);

                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if(await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure(UserErrors.InvalidCode);

            if(user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);


            var code = request.Code;

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException) 
            {
                return Result.Failure(UserErrors.InvalidCode);  
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, DefaultRole.Member);
                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ResndConfirmationEmailAsync(ResendConfirmationEmailRequest request) 
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DuplicatedConfirmation);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code : {code}", code);

            await SendEmail(user, code);

            return Result.Success();
        }

        public async Task<Result> ForgetPasswordAsync(string email) 
        {
            if(await _userManager.FindByEmailAsync(email) is not { } user)
                return Result.Success();

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Reset code : {code}", code);

            await SendResetPasswordEmail(user, code);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request) 
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user == null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);

            IdentityResult result;

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException) 
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if(result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
        }

        private static string GenerateRefreshToken() 
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private async Task SendEmail(ApplicationUser user , string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
                templateModel: new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName },
                        {"{{action_url}}",$"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}"}
                }
                );

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket : Confirmation Email", emailBody));
            await Task.CompletedTask;
        }

        private async Task SendResetPasswordEmail(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
                templateModel: new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName },
                        {"{{action_url}}",$"{origin}/auth/forgetPassword?email={user.Email}&code={code}"}
                }
                );

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket : Reset Password Email", emailBody));
            await Task.CompletedTask;
        }

        private async Task<(IEnumerable<string> userRoles , IEnumerable<string> userPermission)> GetUserRolesAndPermission(ApplicationUser user,CancellationToken cancellation) 
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var userPermissions = await _context.Roles
                .Join(_context.RoleClaims,
                role => role.Id,
                claim => claim.RoleId,
                (role, claim) => new { role, claim }
                )
                .Where(x => userRoles.Contains(x.role.Name!))
                .Select(x => x.claim.ClaimValue!)
                .Distinct()
                .ToListAsync(cancellation);

            return (userRoles, userPermissions);
        }


    }
}
