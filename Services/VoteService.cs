using Microsoft.EntityFrameworkCore;
using Survey_Basket.Contracts.Questions;
using Survey_Basket.Contracts.Votes;

namespace Survey_Basket.Services
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellation = default)
        {
            var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellation);

            if (hasVote)
                return Result.Failure(VoteErrors.DuplicateVote);

            var pollIsExisit = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken: cancellation);
            
            if (!pollIsExisit)
                return Result.Failure(PollErrors.PollNotFound);

            var avaliableQuestions = await _context.Questions
                .Where(x => x.PollId == pollId && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken: cancellation);

            if (!request.Answers.Select(a => a.QuestionId).SequenceEqual(avaliableQuestions))
                return Result.Failure(VoteErrors.InvalidVote);

            var vote = new Vote
            {
                PollId = pollId,
                UserId = userId,
                VoteAnswers = request.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
            };

            await _context.AddAsync(vote,cancellation);

            await _context.SaveChangesAsync(cancellation);

            return Result.Success();
        }
    }
}
