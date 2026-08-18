using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Create([FromBody] CreateRatingRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Dummy User ID
            var rating = await _ratingService.CreateAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = rating.Id }, rating);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var rating = await _ratingService.GetByIdAsync(id, cancellationToken);
            if (rating == null)
            {
                return NotFound($"Rating with ID {id} was not found.");
            }

            return Ok(rating);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RatingResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var ratings = await _ratingService.GetAllAsync(cancellationToken);
            return Ok(ratings);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRatingRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            try
            {
                var isUpdated = await _ratingService.UpdateAsync(id, request, userId, cancellationToken);
                return Ok(isUpdated);
            }
            catch (Exception ex) when (ex.Message == "Rating not found.")
            {
                return NotFound($"Rating with ID {id} was not found.");
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var isDeleted = await _ratingService.DeleteAsync(id, cancellationToken);
                return Ok(isDeleted);
            }
            catch (Exception ex) when (ex.Message == "Rating not found.")
            {
                return NotFound($"Rating with ID {id} was not found.");
            }
        }
    }
}