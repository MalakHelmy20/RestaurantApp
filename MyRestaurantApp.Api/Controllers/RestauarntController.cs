using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Create([FromBody] CreateRestaurantRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Dummy User ID
            //incremental ?
            
            var restaurant = await _restaurantService.CreateAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var restaurants = await _restaurantService.GetAllAsync(cancellationToken);
            return Ok(restaurants);
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(IEnumerable<RestaurantSummary>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered([FromQuery] RestaurantFilterRequest filter, CancellationToken cancellationToken)
        {
            var filtered = await _restaurantService.GetFilteredAsync(filter, cancellationToken);
            return Ok(filtered);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRestaurantRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            try
            {
                var isUpdated = await _restaurantService.UpdateAsync(id, request, userId, cancellationToken);
                return Ok(isUpdated);
            }
            catch (Exception ex) when (ex.Message == "Restaurant not found.")
            {
                return NotFound($"Restaurant with ID {id} was not found.");
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var isDeleted = await _restaurantService.DeleteAsync(id, cancellationToken);
                return Ok(isDeleted);
            }
            catch (Exception ex) when (ex.Message == "Restaurant not found.")
            {
                return NotFound($"Restaurant with ID {id} was not found.");
            }
        }
    }
}