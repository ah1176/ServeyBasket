using Survey_Basket.Contracts.Answers;
using Survey_Basket.Contracts.Common;
using Survey_Basket.Contracts.Questions;
using System.Linq.Dynamic.Core;

namespace Survey_Basket.Services
{
    public class QuestionService(ApplicationDbContext context) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;


        public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId,RequestFilters request, CancellationToken cancellation)
        {
            var isExisitingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellation);

            if (!isExisitingPoll)
                return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollsNotFound);

            var query = _context.Questions
                            .Where(x => x.PollId == pollId);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                query = query.Where(x => x.Content.Contains(request.SearchValue));
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }
                        var source= query.Include(x => x.Answers)
                            .ProjectToType<QuestionResponse>()
                            .AsNoTracking();


            var questions = await PaginatedList<QuestionResponse>.CreateAsync(source, request.PageNumer, request.PageSize);               

            return Result.Success(questions);

            //.Select(x => new QuestionResponse
            //(
            //    x.Id,
            //    x.Content,
            //    x.Answers.Select(a => new AnswerResponse(a.Id, a.Content))

            //))
        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvaliableAsync(int pollId, string userId, CancellationToken cancellation)
        {
            var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellation);

            if (hasVote)
                return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicateVote);

            var pollIsExisit = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken: cancellation);

            if (!pollIsExisit)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var question = await _context.Questions
                            .Where(x => x.PollId == pollId && x.IsActive)
                            .Include(x => x.Answers)
                            .Select(x => new QuestionResponse(
                                x.Id,
                                x.Content,
                                x.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id , a.Content))
                            )).AsNoTracking()
                              .ToListAsync(cancellation);
            return Result.Success<IEnumerable<QuestionResponse>>(question);
        }
        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int QuestionId, CancellationToken cancellation)
        {
            var isExisitingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellation);

            if (!isExisitingPoll)
                return Result.Failure<QuestionResponse>(PollErrors.PollsNotFound);

            var question = await _context.Questions
                            .Where(x => x.Id == QuestionId && x.PollId == pollId)
                            .Include(x => x.Answers)
                            .ProjectToType<QuestionResponse>()
                            .AsNoTracking()
                            .SingleOrDefaultAsync(cancellation);

            if(question is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            return Result.Success(question);
        }
        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellation)
        {
            var isExisitingPoll = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellation);

            if (!isExisitingPoll)
                return Result.Failure<QuestionResponse>(PollErrors.PollsNotFound);

            var isExisitingQuestion = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId, cancellationToken: cancellation);

            if (isExisitingQuestion)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicateQuestionContent);

            var question = request.Adapt<Question>();

            question.PollId = pollId;

            await _context.AddAsync(question, cancellation);

            await _context.SaveChangesAsync(cancellation);

            return Result.Success(question.Adapt<QuestionResponse>());
        }
        public async Task<Result> UpdateAsync(int pollId, int QuestionId, QuestionRequest request, CancellationToken cancellation)
        {
            var isExisitingQuestion = await _context.Questions.AnyAsync(
                x => x.Id != QuestionId
                && x.PollId == pollId
                &&x.Content == request.Content
                , cancellationToken: cancellation);

            if(isExisitingQuestion)
                   return Result.Failure(QuestionErrors.DuplicateQuestionContent);

            var question = await _context.Questions
                .Include(x => x.Answers)
                .SingleOrDefaultAsync(x => x.Id == QuestionId && x.PollId == pollId, cancellationToken: cancellation);

            if(question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.Content = request.Content;

            var currentAnswer = question.Answers.Select(x => x.Content).ToList();

            var newAnswers = request.Answers.Except(currentAnswer).ToList();

            newAnswers.ForEach(x =>
            {
                question.Answers.Add(new Answer { Content = x });
            });

            question.Answers.ToList().ForEach(x =>
            {
                x.IsActive = request.Answers.Contains(x.Content);
            });

            await _context.SaveChangesAsync(cancellation);

            return Result.Success();
        }
        public async Task<Result> ToggleAsync(int pollId, int QuestionId, CancellationToken cancellation)
        {
            var question = await _context.Questions.SingleOrDefaultAsync(x => x.Id == QuestionId && x.PollId == pollId , cancellation);

            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.IsActive = !question.IsActive;

            await _context.SaveChangesAsync(cancellation);

            return Result.Success();

        }


    }
}
