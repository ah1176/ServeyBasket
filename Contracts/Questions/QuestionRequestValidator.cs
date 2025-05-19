using FluentValidation;

namespace Survey_Basket.Contracts.Questions
{
    public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionRequestValidator() 
        {
            RuleFor(x => x.Content)
                .NotEmpty();
            
            RuleFor(x => x.Content)
                .Length(3,100);

            RuleFor(x => x.Answers)
                .NotNull();

            RuleFor(x => x.Answers)
                .Must(x => x.Count > 1)
                .WithMessage("question must has at least 2 values")
                .When(x => x.Answers != null);

            RuleFor(x => x.Answers)
              .Must(x => x.Distinct().Count() == x.Count)
              .WithMessage("you cannot add duplicate answers in the same question ")
              .When(x => x.Answers != null);
        }
    }
}
