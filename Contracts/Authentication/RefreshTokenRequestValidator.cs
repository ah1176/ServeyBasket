﻿using FluentValidation;

namespace Survey_Basket.Contracts.Authentication
{
    public class RefreshTokenRequestValidator:AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator() 
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
