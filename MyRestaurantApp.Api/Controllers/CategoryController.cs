using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Application.Features.Categories.Services;
using MyRestaurantApp.Application.Features.Restaurants.Services;

namespace MyRestaurantApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IRestaurantService _restaurantService;

        public CategoryController(ICategoryService categoryService, IRestaurantService restaurantService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _restaurantService = restaurantService ?? throw new ArgumentNullException(nameof(restaurantService));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

         
            var restaurant = await _restaurantService.GetByIdAsync(request.RestaurantId, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Associated restaurant with ID {request.RestaurantId} was not found.");
            }

          
            if (!User.IsInRole("SystemAdmin") && restaurant.Owner?.Id != userIdGuid)
            {
                return Forbid();
            }

            var category = await _categoryService.CreateAsync(request, userIdGuid, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return NotFound($"Category with ID {id} was not found.");
            }

            return Ok(category);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllAsync(cancellationToken);
            return Ok(categories);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return NotFound($"Category with ID {id} was not found.");
            }

          
            var restaurant = await _restaurantService.GetByIdAsync(request.RestaurantId, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Associated restaurant with ID {request.RestaurantId} was not found.");
            }

            if (!User.IsInRole("SystemAdmin") && restaurant.Owner?.Id != userIdGuid)
            {
                return Forbid();
            }

            var isUpdated = await _categoryService.UpdateAsync(id, request, userIdGuid, cancellationToken);
            if (!isUpdated)
            {
                return NotFound($"Category with ID {id} was not found.");
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var category = await _categoryService.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                return NotFound($"Category with ID {id} was not found.");
            }

            var restaurant = await _restaurantService.GetByIdAsync(category.RestaurantId, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Associated restaurant with ID {category.RestaurantId} was not found.");
            }

            if (!User.IsInRole("SystemAdmin") && restaurant.Owner?.Id != userIdGuid)
            {
                return Forbid();
            }

            var isDeleted = await _categoryService.DeleteAsync(id, cancellationToken);
            if (!isDeleted)
            {
                return NotFound($"Category with ID {id} was not found.");
            }

            return NoContent();
        }
    }
}