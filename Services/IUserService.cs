using Survey_Basket.Contracts.Users;

namespace Survey_Basket.Services
{
    public interface IUserService
    {
        Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
        Task<Result<UserResponse>> GetAsync(string userId);

        Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellation = default);
        Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateProfileAsync(string userId , UpdateProfileRequest request);
        Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

        Task<Result> ToggleStatusAsync(string userId, CancellationToken cancellationToken);
        Task<Result> UnLockAsync(string userId);

         Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    }
}
