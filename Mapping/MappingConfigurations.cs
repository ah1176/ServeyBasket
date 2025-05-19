using Survey_Basket.Contracts.Questions;
using Survey_Basket.Contracts.Users;

namespace Survey_Basket.Mapping
{
    public class MappingConfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<QuestionRequest, Question>()
                .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));

            config.NewConfig<(ApplicationUser user , IList<string> roles), UserResponse>()
                .Map(dest => dest, src => src.user)
                .Map(dest => dest.Roles, src => src.roles);
        }
    }
}
