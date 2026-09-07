using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Application.Features.Users.Services;

namespace MyRestaurantApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.LoginAsync(request, cancellationToken);
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.RegisterAsync(request, cancellationToken);
            if (response == null)
            {
                return BadRequest(new { message = "Registration failed." });
            }

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] string? role, CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(role ,cancellationToken);
            return Ok(users);
        }

        [HttpGet("{userId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var createdByUserId))
            {
                return Unauthorized();
            }

            var result = await _userService.CreateAsync(request, createdByUserId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { userId = result.Id }, result);
        }

        [HttpPut("{userId:guid}")]
        [Authorize]
         [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var updatedByUserId))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("SystemAdmin");
            if (!isAdmin && userId != updatedByUserId)
            {
                return Forbid();
            }

            var result = await _userService.UpdateAsync(userId, request, updatedByUserId, cancellationToken);
            if (!result)
            {
                return NotFound(new { message = "User not found." });
            }

            return NoContent() ;
        }
       
        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteAsync(userId, cancellationToken);
            if (!result)
            {
                return NotFound(new { message = "User not found." });
            }

            return NoContent();
        }
    }
}