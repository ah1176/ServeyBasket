using Survey_Basket.Contracts.Common;
using Survey_Basket.Contracts.Questions;

namespace Survey_Basket.Services
{
    public interface IQuestionService
    {
        Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilters request, CancellationToken cancellation);
        Task<Result<IEnumerable<QuestionResponse>>> GetAvaliableAsync(int pollId,string userId, CancellationToken cancellation);
        Task<Result<QuestionResponse>> GetAsync(int pollId ,int QuestionId, CancellationToken cancellation);
        Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellation);
        Task<Result> UpdateAsync(int pollId, int QuestionId, QuestionRequest request, CancellationToken cancellation);

        Task<Result> ToggleAsync(int pollId, int QuestionId, CancellationToken cancellation);

    }
}
