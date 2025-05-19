using Microsoft.AspNetCore.Authorization;

namespace Survey_Basket.Authentication.Filters
{
    public class PermissionAuthrizationPolicyProvider(IOptions<AuthorizationOptions> options)
        :DefaultAuthorizationPolicyProvider(options)
    {
        private readonly AuthorizationOptions _options = options.Value;

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var polciy = await base.GetPolicyAsync(policyName);

            if (polciy != null)
                return polciy;

            var permissionPolicy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            _options.AddPolicy(policyName, permissionPolicy);

            return permissionPolicy;
        }
    }
}
