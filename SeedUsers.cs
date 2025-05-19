using Microsoft.AspNetCore.Identity;

namespace Survey_Basket
{
    public class SeedUsers
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminEmail = "admin@survey-basket.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null && string.IsNullOrEmpty(adminUser.PasswordHash))
            {
                string newPassword = DefaultUsers.AdminPassword;
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, newPassword);

                await userManager.UpdateAsync(adminUser);
                Console.WriteLine("✅ Admin password has been updated.");
            }
        }
    }
}
