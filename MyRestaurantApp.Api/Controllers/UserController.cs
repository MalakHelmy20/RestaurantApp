using Microsoft.AspNetCore.Mvc;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Application.Features.Users.Services;
using MyRestaurantApp.Application.Features.Users.Mapping;
using MyRestaurantApp.Application.Features.Users.IRepository;
using MyRestaurantApp.Domain;
using Microsoft.AspNetCore.Authorization;

namespace MyRestaurantApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.LoginAsync(request, cancellationToken);
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.RegisterAsync(request, cancellationToken);
            if (response == null)
            {
                return BadRequest(new { message = "Registration failed." });
            }
            return Ok(response);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetById(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [Authorize(Roles= "SystemAdmin")]
        [HttpDelete("delete/{userId:guid}")]
        public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteAsync(userId, cancellationToken);
            if (!result)
                return NotFound();

            return Ok();
        }

        [Authorize(Roles= "SystemAdmin")]
        [HttpPut("update/{userId:guid}")]
        public async Task<IActionResult> Update(
            Guid userId, 
            [FromBody] UpdateUserRequest request, 
            [FromQuery] Guid updatedByUserId,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateAsync(userId, request, updatedByUserId, cancellationToken);
            if (!result)
                return NotFound();

            return Ok(true);
        }

        
        [Authorize(Roles= "SystemAdmin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserRequest request,
            [FromQuery] Guid createdByUserId,
            CancellationToken cancellationToken)
        {
            if (request == null)
                return BadRequest(new { message = "Data cannot be null." });

            var result = await _userService.CreateAsync(request, createdByUserId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { userId = result.Id }, result);
        }
    }
}
  
