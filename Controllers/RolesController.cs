using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey_Basket.Abstractions;
using Survey_Basket.Contracts.Roles;

namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        [HttpGet("")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled , CancellationToken cancellation) 
        {
            var roles = await _roleService.GetAllAsync(includeDisabled,cancellation);

            return Ok(roles);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _roleService.GetRole(id);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("")]
        [HasPermission(Permissions.AddRoles)]
        public async Task<IActionResult> AddRole([FromBody] RoleRequest request)
        {
            var result = await _roleService.AddRoleAsync(request);


            return result.IsSuccess ? CreatedAtAction(nameof(Get), new {result.Value.Id} , result.Value) : result.ToProblem();

        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> UpdateRole([FromRoute] string id,[FromBody] RoleRequest request)
        {
            var result = await _roleService.UpdateRoleAsync(id,request);


            return result.IsSuccess ? NoContent() : result.ToProblem();

        }

        [HttpPut("{id}/toggle")]
        [HasPermission(Permissions.UpdateRoles)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string id)
        {
            var result = await _roleService.ToggleStatusAsync(id);


            return result.IsSuccess ? NoContent() : result.ToProblem();

        }
    }
}
