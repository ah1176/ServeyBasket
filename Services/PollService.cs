
namespace Survey_Basket.Services
{
    public class PollService(ApplicationDbContext context) : IPollService
    {
        private readonly ApplicationDbContext _context = context ;
         
        public async Task<Result<IEnumerable<PollResponse>>> GetAllAsync(CancellationToken cancellation = default)
        {
            var polls = await _context.Polls.AsNoTracking().ToListAsync(cancellation);

            return !polls.Any() ? Result.Failure<IEnumerable<PollResponse>>(PollErrors.PollsNotFound) : Result.Success(polls.Adapt<IEnumerable<PollResponse>>());
        }
        public async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellation = default)
        {
            var currentPolls = await _context.Polls
                             .Where(x  => x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
                             .AsNoTracking()
                             .ToListAsync(cancellation);

            return currentPolls.Any()
                ?Result.Success(currentPolls.Adapt<IEnumerable<PollResponse>>())
                : Result.Failure<IEnumerable<PollResponse>>(PollErrors.PollsNotFound);
        }

        public async Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            var poll = await _context.Polls.FindAsync(id, cancellation);
            
            return poll is null ? Result.Failure<PollResponse>(PollErrors.PollNotFound) : Result.Success(poll.Adapt<PollResponse>());
        }

        public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellation = default)
        {
            var isExisting = await _context.Polls.AnyAsync(x => x.Title == request.Title);

            if (isExisting)
              return  Result.Failure<PollResponse>(PollErrors.DuplicatePollTitle);

            var poll = request.Adapt<Poll>();

            await _context.Polls.AddAsync(poll, cancellation);
            await _context.SaveChangesAsync(cancellation);

            return Result.Success(poll.Adapt<PollResponse>());
        }

        public async Task<Result> UpdateAsync(int id, PollRequest request , CancellationToken cancellation = default)
        {
            var isExisting = await _context.Polls.AnyAsync(x => x.Title == request.Title && x.Id != id, cancellationToken: cancellation);

            if (isExisting)
                return Result.Failure<PollResponse>(PollErrors.DuplicatePollTitle);

            var currentPoll = await _context.Polls.FindAsync(id,cancellation);

            if (currentPoll is null)
                return Result.Failure(PollErrors.PollNotFound);

            currentPoll.Title = request.Title;

            currentPoll.Summary = request.Summary;

            currentPoll.StartsAt = request.StartsAt;

            currentPoll.EndsAt = request.EndsAt;

            await _context.SaveChangesAsync(cancellation);

            return Result.Success();

        }

        public async Task<Result> DeleteAsync(int id , CancellationToken cancellation)
        {
            var poll = await _context.Polls.FindAsync(id , cancellation);

            if (poll is null)
                return Result.Failure(PollErrors.PollNotFound);

            _context.Remove(poll);

            await _context.SaveChangesAsync(cancellation);

            return Result.Success();
        }
        public async Task<Result> TogglePublishedStatusAsync(int id, CancellationToken cancellation = default)
        {
            var poll = await _context.Polls.FindAsync(id, cancellation);

            if (poll is null)
                return Result.Failure(PollErrors.PollNotFound);

            poll.IsPublished = !poll.IsPublished;

            await _context.SaveChangesAsync(cancellation);

            return Result.Success();
        }


    }
}
