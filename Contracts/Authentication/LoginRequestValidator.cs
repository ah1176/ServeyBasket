﻿using FluentValidation;

namespace Survey_Basket.Contracts.Authentication
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Passowrd)
              .NotEmpty();
              
        }
    }
}
