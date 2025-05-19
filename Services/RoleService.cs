using Microsoft.AspNetCore.Identity;
using Survey_Basket.Abstractions.Consts;
using Survey_Basket.Contracts.Roles;

namespace Survey_Basket.Services
{
    public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext context) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false , CancellationToken cancellation = default)=>
            await _roleManager.Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted ||(includeDisabled.HasValue  && includeDisabled.Value)))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellation);

        public async Task<Result<RoleDetailsResponse>> GetRole(string id) 
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

            var permissions = await _roleManager.GetClaimsAsync(role);

            var response = new RoleDetailsResponse(role.Id,role.Name!,role.IsDeleted,permissions.Select(x => x.Value));

            return Result.Success(response);
        }


        public async Task<Result<RoleDetailsResponse>> AddRoleAsync(RoleRequest request)
        {
            var roleIsExisit = await _roleManager.RoleExistsAsync(request.Name);

            if(roleIsExisit)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.DuplicatedRole);

            var allowedPermissions = Permissions.GetAllPermissions();

            if(request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermissions);

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded) 
            {
                var permissions = request.Permissions
                    .Select(x => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id,
                    });

                await _context.AddRangeAsync(permissions);

                await _context.SaveChangesAsync();

                var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, request.Permissions);

                return Result.Success(response);
            }

            var errors = result.Errors.First();


            return Result.Failure<RoleDetailsResponse>(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> UpdateRoleAsync(string id,RoleRequest request)
        {
            var roleIsExisit = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);

            if (roleIsExisit)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.DuplicatedRole);

            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

            var allowedPermissions = Permissions.GetAllPermissions();

            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermissions);

           role.Name = request.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                var currentPermissions = await _context.RoleClaims
                    .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                    .Select(x => x.ClaimValue)
                    .ToListAsync();

                var newPermissions = request.Permissions.Except(currentPermissions)
                    .Select(x => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = x,
                        RoleId = role.Id,
                    });

                var removedPermissions = currentPermissions.Except(request.Permissions);

                await _context.RoleClaims
                    .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();


                await _context.AddRangeAsync(newPermissions);

                await _context.SaveChangesAsync();

               return Result.Success();
            }

            var errors = result.Errors.First();


            return Result.Failure<RoleDetailsResponse>(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> ToggleStatusAsync(string id) 
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure(RoleErrors.RoleNotFound);

            role.IsDeleted = !role.IsDeleted;

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
