using FluentValidation;

namespace Survey_Basket.Contracts.Polls
{
    public class PollRequestValidator : AbstractValidator<PollRequest>
    {
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("title should have value")
                .Length(3, 100);

            RuleFor(x => x.Summary)
                .NotEmpty()
                .Length(15, 1500);

            RuleFor(x => x.StartsAt)
                .NotEmpty()
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

            RuleFor(x => x.EndsAt)
                .NotEmpty()
                .GreaterThanOrEqualTo(x => x.StartsAt);
        }
    }
}
