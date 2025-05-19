using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey_Basket.Contracts.Votes;
using Survey_Basket.Extensions;
using System.Security.Claims;

namespace Survey_Basket.Controllers
{
    [Route("api/polls/{pollId}/vote")]
    [ApiController]
    [Authorize(Roles = DefaultRole.Member)]
    public class VotesController(IQuestionService questionService , IVoteService voteService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;
        private readonly IVoteService _voteService = voteService;

        [HttpGet("")]
        public async Task<IActionResult> Start([FromRoute] int pollId , CancellationToken cancellation) 
        {
            var userId = User.GetUserId();

            var result = await _questionService.GetAvaliableAsync(pollId, userId!, cancellation);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("")]
        public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellation)
        {
            var result = await _voteService.AddAsync(pollId,User.GetUserId()!,request);

            return result.IsSuccess
                ? Created()
                : result.ToProblem();   
        }
    }
}
