
namespace Survey_Basket.Errors
{
    public static class RoleErrors
    {
        public static readonly Error RoleNotFound = new("Not Found", "Role Not Found", StatusCodes.Status404NotFound);
        public static readonly Error InvalidPermissions = new("Invalid Permissions", "Invalid Permissions", StatusCodes.Status400BadRequest);
        public static readonly Error DuplicatedRole = new("Duplicated Role", "Cannot Insert Duplicate Role", StatusCodes.Status409Conflict);
    }
}
