using Application.DTO.Request;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;

namespace EventManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UserExceptionFilter]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
        {
                var userResponse = await _userService.GetUserByIdAsync(id, cancellationToken);
                
                return Ok(userResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest userRequest, CancellationToken cancellationToken)
        {
                var userResponse = await _userService.CreateUserAsync(userRequest, cancellationToken);
                return CreatedAtAction(nameof(GetUserById), new { id = userResponse.Id }, userResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequest userRequest, CancellationToken cancellationToken)
        {
           
                await _userService.UpdateUserAsync(id, userRequest, cancellationToken);
                return NoContent();
           
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
        {
           
                await _userService.DeleteUserAsync(id, cancellationToken);
                return NoContent();
           
        }
    }
}
