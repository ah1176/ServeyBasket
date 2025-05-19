using Microsoft.AspNetCore.Authorization;

namespace Survey_Basket.Authentication.Filters
{
    public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
    {
    }
}
