using Survey_Basket.Contracts.Results;

namespace Survey_Basket.Services
{
    public interface IResultService
    {
        Task<Result<PollVotesResponse>> GetPollVotesAsync(int  pollId , CancellationToken cancellation = default);
        Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellation = default);
        Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellation = default);
    }
}
