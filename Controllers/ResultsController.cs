using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Survey_Basket.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [HasPermission(Permissions.Results)]
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        [HttpGet("raw-data")]
        public async Task<IActionResult> GetPollVotes(int pollId , CancellationToken cancellation)
        {
            var result = await _resultService.GetPollVotesAsync(pollId,cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("votes-per-day")]
        public async Task<IActionResult> GetVotesPerDay(int pollId, CancellationToken cancellation)
        {
            var result = await _resultService.GetVotesPerDayAsync(pollId, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("votes-per-question")]
        public async Task<IActionResult> GetVotesPerQuestion(int pollId, CancellationToken cancellation)
        {
            var result = await _resultService.GetVotesPerQuestionAsync(pollId, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}
