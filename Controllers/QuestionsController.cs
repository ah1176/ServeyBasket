using Microsoft.AspNetCore.Authorization;
using Survey_Basket.Contracts.Common;
using Survey_Basket.Contracts.Questions;

namespace Survey_Basket.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
   
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        [HttpGet("")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetAll([FromRoute] int pollId, [FromQuery] RequestFilters filters, CancellationToken cancellation)
        {
            var result = await _questionService.GetAllAsync(pollId,filters, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("{Id}")]
        [HasPermission(Permissions.GetQuestions)]

        public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int Id ,CancellationToken cancellation) 
        {
            var result = await _questionService.GetAsync(pollId, Id, cancellation);

            return result.IsSuccess ? Ok(result.Value):result.ToProblem();
        }

        [HttpPost("")]
        [HasPermission(Permissions.AddQuestions)]
        public async Task<IActionResult> Add([FromRoute] int pollId , [FromBody] QuestionRequest request , CancellationToken cancellation )
        {
            var result = await _questionService.AddAsync( pollId , request , cancellation );

            return result.IsSuccess ?
                CreatedAtAction(nameof(Get), new { pollId, result.Value.Id }, result.Value) :
                result.ToProblem();
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> Update([FromRoute] int pollId,[FromRoute] int id , [FromBody] QuestionRequest request, CancellationToken cancellation)
        {
            var result = await _questionService.UpdateAsync(pollId, id, request, cancellation);

            return result.IsSuccess ?
                NoContent() :
                result.ToProblem();
        }

        [HttpPut("{id}/toggleStatus")]
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> TogglePublish([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellation)
        {
            var result = await _questionService.ToggleAsync(pollId,id, cancellation);

            return result.IsSuccess
                 ? NoContent()
                 : result.ToProblem();
        }
    }
}
