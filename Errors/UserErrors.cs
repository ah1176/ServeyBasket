
namespace Survey_Basket.Errors
{
    public static class UserErrors
    {
        public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status400BadRequest);
        public static readonly Error DisabledUser = new("User.DisabledUser", "Disabled user please contact your admin ", StatusCodes.Status400BadRequest);

        public static readonly Error DuplicateEmail = new("User.DuplicateEmail", "another user with this email is exisit", StatusCodes.Status409Conflict);

        public static readonly Error EmailNotConfirmed = new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidCode = new("User.InvalidCode", "Invalid Code", StatusCodes.Status400BadRequest);

        public static readonly Error DuplicatedConfirmation = new("User.DuplicatedConfirmation", "Email alredy confirmed", StatusCodes.Status409Conflict);
        public static readonly Error LockedOut = new("User.Locked Out", "User Locked Out", StatusCodes.Status401Unauthorized);

        public static readonly Error InvalidRefreshToken = new("User.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status400BadRequest);
        public static readonly Error UserNotFound = new("User.NotFound", "User was not found", StatusCodes.Status404NotFound);
        public static readonly Error InvalidRoles = new("User.InvalidRoles", "Invalid Roles", StatusCodes.Status400BadRequest);
    }
}
