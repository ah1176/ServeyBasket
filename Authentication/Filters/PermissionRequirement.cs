using Microsoft.AspNetCore.Authorization;

namespace Survey_Basket.Authentication.Filters
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
