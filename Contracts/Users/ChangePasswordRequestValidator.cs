﻿using FluentValidation;
using Survey_Basket.Abstractions.Consts;

namespace Survey_Basket.Contracts.Users
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
               .Matches(RegexPattern.Password)
               .WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase")
               .NotEqual(x => x.CurrentPassword)
               .WithMessage("New Password cannot be the same as current password");
        }
    }
}
