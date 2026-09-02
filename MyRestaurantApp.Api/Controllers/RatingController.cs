using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurantApp.Application.Features.Ratings.Dtos;
using MyRestaurantApp.Application.Features.Ratings.Services;

namespace MyRestaurantApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateRatingRequest request, CancellationToken cancellationToken)
        {
            // 1. Extract logged-in user ID from Claims
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            // 2. Create Rating
            var rating = await _ratingService.CreateAsync(request, userIdGuid, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var rating = await _ratingService.GetByIdAsync(id, cancellationToken);
            if (rating == null)
            {
                return NotFound($"Rating with ID {id} was not found.");
            }

            return Ok(rating);
        }

        [HttpGet("restaurant/{restaurantId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<RatingResponse>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRestaurantId(Guid restaurantId, CancellationToken cancellationToken)
        {
            var ratings = await _ratingService.GetByRestaurantIdAsync(restaurantId, cancellationToken);
            return Ok(ratings);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RatingResponse>), StatusCodes.Status200OK)]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var ratings = await _ratingService.GetAllAsync(cancellationToken);
            return Ok(ratings);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRatingRequest request, CancellationToken cancellationToken)
        {
            // 1. Extract and validate user ID from JWT claims
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            // 2. Retrieve existing rating to verify existence
            var existingRating = await _ratingService.GetByIdAsync(id, cancellationToken);
            if (existingRating == null)
            {
                return NotFound($"Rating with ID {id} was not found.");
            }

            // 3. Ownership check: Ensure the customer can only update their own rating
            if (existingRating.UserId != userIdGuid)
            {
                return Forbid();
            }

            // 4. Perform update operation
            var isUpdated = await _ratingService.UpdateAsync(id, request, userIdGuid, cancellationToken);
            if (!isUpdated)
            {
                return NotFound($"Rating with ID {id} was not found.");
            }

            return Ok(isUpdated);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SystemAdmin,Customer")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            // 1. Extract and validate user ID from JWT claims
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            // 2. Retrieve existing rating to verify existence
            var existingRating = await _ratingService.GetByIdAsync(id, cancellationToken);
            if (existingRating == null)
            {
                return NotFound($"Rating with ID {id} was not found.");
            }

            // 3. Ownership check: Customers can only delete their own ratings; SystemAdmin can delete any
            if (!User.IsInRole("SystemAdmin") && existingRating.UserId != userIdGuid)
            {
                return Forbid();
            }

            // 4. Perform delete operation
            var isDeleted = await _ratingService.DeleteAsync(id, cancellationToken);
            if (!isDeleted)
            {
                return NotFound($"Rating with ID {id} was not found.");
            }

            return Ok(isDeleted);
        }
    }
}