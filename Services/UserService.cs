using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Survey_Basket.Contracts.Users;
using System.Reflection.Metadata.Ecma335;

namespace Survey_Basket.Services
{
    public class UserService(UserManager<ApplicationUser> userManager
        , ApplicationDbContext context
        , IRoleService roleService) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        private readonly IRoleService _roleService = roleService;

        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellation = default) =>
            await (from u in _context.Users
                   join ur in _context.UserRoles
                   on u.Id equals ur.UserId
                   join r in _context.Roles
                   on ur.RoleId equals r.Id into roles
                   where !roles.Any(x => x.Name == DefaultRole.Member)
                   select new
                   {
                       u.Id,
                       u.FirstName,
                       u.LastName,
                       u.Email,
                       u.IsDisabled,
                       Roles = roles.Select(x => x.Name!).ToList()
                   }
                   )
            .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
            .Select
            (u => new UserResponse(
                 u.Key.Id,
                 u.Key.FirstName,
                 u.Key.LastName,
                 u.Key.Email,
                 u.Key.IsDisabled,
                 u.SelectMany(x => x.Roles)
            ))
            .ToListAsync(cancellation);


        public async Task<Result<UserResponse>> GetAsync(string userId)
        {
            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return Result.Failure<UserResponse>(UserErrors.UserNotFound);

            var userRoles = await _userManager.GetRolesAsync(user);

            var respone = (user, userRoles).Adapt<UserResponse>();

            return Result.Success(respone);
        }


        public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var emailExisits = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailExisits)
                return Result.Failure<UserResponse>(UserErrors.DuplicateEmail);

            var allowedRoles = await _roleService.GetAllAsync(cancellation: cancellationToken);

            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

            var user = request.Adapt<ApplicationUser>();

            user.UserName = request.Email;
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, request.Roles);

                var respone = (user, request.Roles).Adapt<UserResponse>();

                return Result.Success(respone);
            }

            var error = result.Errors.First();

            return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var emailExisits = await _userManager.Users.AnyAsync(x => x.Email == request.Email && x.Id != userId, cancellationToken);

            if (emailExisits)
                return Result.Failure(UserErrors.DuplicateEmail);

            var allowedRoles = await _roleService.GetAllAsync(cancellation: cancellationToken);

            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure(UserErrors.InvalidRoles);

            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            user = request.Adapt(user);

            user.UserName = request.Email;
            user.NormalizedUserName = request.Email.ToUpper();

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                await _context.UserRoles
                    .Where(x => x.UserId == userId)
                    .ExecuteDeleteAsync(cancellationToken);

                await _userManager.AddToRolesAsync(user, request.Roles);

                return Result.Success();
            }

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ToggleStatusAsync(string userId, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            user.IsDisabled = !user.IsDisabled;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> UnLockAsync(string userId)
        {
            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
    

        public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == userId)
                .ProjectToType<UserProfileResponse>()
                .SingleAsync();

            return Result.Success(user);
        }

        public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            //var user = await _userManager.FindByIdAsync(userId);

            //user = request.Adapt(user);

            //await _userManager.UpdateAsync(user!);

            await _userManager.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(x => x.FirstName, request.FirstName)
                           .SetProperty(x => x.LastName, request.LastName)
                );

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request) 
        {
            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
    }
}
