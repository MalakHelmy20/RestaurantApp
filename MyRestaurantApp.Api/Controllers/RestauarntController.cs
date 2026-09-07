using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurantApp.Application.Features.Restaurants.Dtos;
using MyRestaurantApp.Application.Features.Restaurants.Services;

namespace MyRestaurantApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Create([FromBody] CreateRestaurantRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            if (!User.IsInRole("SystemAdmin"))
            {
                request.OwnerId = userIdGuid;
            }

            var restaurant = await _restaurantService.CreateAsync(request, userIdGuid, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {id} was not found.");
            }

            return Ok(restaurant);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RestaurantResponse>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var restaurants = await _restaurantService.GetAllAsync(cancellationToken);
            return Ok(restaurants);
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(IEnumerable<RestaurantSummary>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetFiltered([FromQuery] RestaurantFilterRequest filter, CancellationToken cancellationToken)
        {
            var filtered = await _restaurantService.GetFilteredAsync(filter, cancellationToken);
            return Ok(filtered);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRestaurantRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var restaurant = await _restaurantService.GetByIdAsync(id, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {id} was not found.");
            }

            // Ownership Check
            if (!User.IsInRole("SystemAdmin") && restaurant.Owner?.Id != userIdGuid)
            {
                return Forbid(); // 403 Forbidden
            }

            var result = await _restaurantService.UpdateAsync(id, request, userIdGuid, cancellationToken);
            if (!result)
            {
                return NotFound($"Restaurant with ID {id} was not found.");
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {id} was not found.");
            }

            var result= await _restaurantService.DeleteAsync(id, cancellationToken);
            if (!result)
            {
                return NotFound($"Restaurant with ID {id} was not found.");
            }

            return NoContent();
        }
    }
}