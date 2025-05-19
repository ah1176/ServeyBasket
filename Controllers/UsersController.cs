using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey_Basket.Contracts.Users;

namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetAll(CancellationToken cancellation) 
        {
            return Ok(await _userService.GetAllAsync(cancellation));
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> Get([FromRoute]string id)
        {
            var result = await _userService.GetAsync(id);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("")]
        [HasPermission(Permissions.AddUsers)]
        public async Task<IActionResult> Add([FromBody] CreateUserRequest request , CancellationToken cancellationToken)
        {
            var result = await _userService.AddAsync(request,cancellationToken);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new {result.Value.Id},result.Value) : result.ToProblem();
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> Add([FromRoute] string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateAsync(id,request, cancellationToken);

            return result.IsSuccess ? NoContent(): result.ToProblem();
        }

        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> Toggle([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _userService.ToggleStatusAsync(id, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}/unlock-user")]
        public async Task<IActionResult> Unlock([FromRoute] string id)
        {
            var result = await _userService.UnLockAsync(id);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
