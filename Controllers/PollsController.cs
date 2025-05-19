
using Microsoft.AspNetCore.Authorization;
using Survey_Basket.Abstractions;



namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var results = await _pollService.GetAllAsync(cancellation);

            return results.IsSuccess
            ? Ok(results.Value)
            : results.ToProblem();
        }

        [HttpGet("current")]
        [Authorize(Roles = DefaultRole.Member)]
        public async Task<IActionResult> Getcurrent(CancellationToken cancellation)
        {
            var results = await _pollService.GetCurrentAsync(cancellation);

            return results.IsSuccess
            ? Ok(results.Value)
            : results.ToProblem();
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellation)
        {
            var result = await _pollService.GetByIdAsync(id, cancellation);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }



        [HttpPost("")]
        [HasPermission(Permissions.AddPolls)]
        public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellation)
        {
            var result = await _pollService.AddAsync(request, cancellation);

            return result.IsSuccess 
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();

        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellation)
        {
            var result = await _pollService.UpdateAsync(id, request, cancellation);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.DeletePolls)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellation)
        {
            var result = await _pollService.DeleteAsync(id, cancellation);

            return result.IsSuccess 
               ? NoContent()
                : result.ToProblem();
        }
        

        [HttpPut("{id}/togglePublish")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellation)
        {
            var result = await _pollService.TogglePublishedStatusAsync(id, cancellation);

           return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
    }

}
