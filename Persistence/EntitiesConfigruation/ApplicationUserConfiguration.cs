﻿


using Microsoft.AspNetCore.Identity;

namespace Survey_Basket.Persistence.EntitiesConfigruation
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName).HasMaxLength(100);
            builder.Property(x => x.LastName).HasMaxLength(100);

            builder.OwnsMany(x => x.RefreshTokens)
                    .ToTable("RefreshTokens")
                    .WithOwner()
                    .HasForeignKey("UserId");

            var passwordHasher = new PasswordHasher<ApplicationUser>();

            builder.HasData( new ApplicationUser 
            {
                Id = DefaultUsers.AdminId,
                FirstName = "Survey Basket",
                LastName = "Admin",
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                EmailConfirmed = true,
                //PasswordHash = passwordHasher.HashPassword(null!,DefaultUsers.AdminPassword)
            });

        }
    }
}
