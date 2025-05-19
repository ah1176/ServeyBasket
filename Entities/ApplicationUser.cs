﻿using Microsoft.AspNetCore.Identity;

namespace Survey_Basket.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public bool IsDisabled { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
