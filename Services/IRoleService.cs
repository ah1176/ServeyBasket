using Survey_Basket.Contracts.Roles;

namespace Survey_Basket.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellation = default);
        Task<Result<RoleDetailsResponse>> GetRole(string id);

        Task<Result<RoleDetailsResponse>> AddRoleAsync(RoleRequest request);
        Task<Result> UpdateRoleAsync(string id, RoleRequest request);

        Task<Result> ToggleStatusAsync(string id);
    }
}
