using Survey_Basket.Contracts.Questions;
using Survey_Basket.Contracts.Results;
using Survey_Basket.Entities;

namespace Survey_Basket.Services
{
    public class ResultService(ApplicationDbContext context) : IResultService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellation = default)
        {
            var pollVotes = await _context.Polls
                            .Where(x => x.Id == pollId)
                            .Select( x => new PollVotesResponse(
                                x.Title,
                                x.Votes.Select(v => new VoteResponse(
                                    $"{v.user.FirstName} {v.user.LastName}",
                                    v.SubmittedOn,
                                    v.VoteAnswers.Select(a => new QuestionAnswerResponse(
                                        a.Question.Content,
                                        a.Answer.Content
                                        ))
                                    ))
                            )).SingleOrDefaultAsync(cancellation);

            return pollVotes is null? Result.Failure<PollVotesResponse>(PollErrors.PollNotFound)
                :Result.Success(pollVotes);
                            
        }

        public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellation = default)
        {
            var isExisitingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellation);

            if (!isExisitingPoll)
                return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollsNotFound);

            var votesPerDay = await _context.Votes
                .Where(x => x.PollId == pollId)
                .GroupBy(x => new { Date = DateOnly.FromDateTime(x.SubmittedOn) })
                .Select(x => new VotesPerDayResponse(
                    x.Key.Date,
                    x.Count()
                    )).ToListAsync(cancellation);

            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
        }

        public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellation = default)
        {
            var isExisitingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellation);

            if (!isExisitingPoll)
                return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollsNotFound);

            var votesPerQuestion = await _context.VoteAnswers
                .Where(v => v.Vote.PollId == pollId)
                .Select(v => new VotesPerQuestionResponse(
                    v.Question.Content,
                    v.Question.Votes
                    .GroupBy(v => new {AnswerId = v.Answer.Id , AnswerContent = v.Answer.Content })
                    .Select(x => new VotesPerAnswersResponse(
                        x.Key.AnswerContent
                        , x.Count()
                        )
                    )
                    )
                ).ToListAsync(cancellation);

            return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
        }
    }
}
