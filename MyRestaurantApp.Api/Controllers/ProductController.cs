using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurantApp.Application.Features.Products.Dtos;
using MyRestaurantApp.Application.Features.Products.Services;
using MyRestaurantApp.Application.Features.Restaurants.Services;

namespace MyRestaurantApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRestaurantService _restaurantService;

        public ProductController(IProductService productService, IRestaurantService restaurantService)
        {
            _productService = productService;
            _restaurantService = restaurantService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
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

            var product = await _productService.CreateAsync(request, userIdGuid, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound($"Product with ID {id} was not found.");
            }

            return Ok(product);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync(cancellationToken);
            return Ok(products);
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> GetFiltered([FromQuery] ProductFilterRequest filter, CancellationToken cancellationToken)
        {
            var products = await _productService.GetFilteredAsync(filter, cancellationToken);
            return Ok(products);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound($"Product with ID {id} was not found.");
            }

            var restaurant = await _restaurantService.GetByIdAsync(product.RestaurantId, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Associated restaurant with ID {product.RestaurantId} was not found.");
            }

            if (!User.IsInRole("SystemAdmin") && restaurant.Owner?.Id != userIdGuid)
            {
                return Forbid();
            }

            var isUpdated = await _productService.UpdateAsync(id, request, userIdGuid, cancellationToken);
            if (!isUpdated)
            {
                return NotFound($"Product with ID {id} was not found.");
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var product = await _productService.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound($"Product with ID {id} was not found.");
            }

            var restaurant = await _restaurantService.GetByIdAsync(product.RestaurantId, cancellationToken);
            if (restaurant == null)
            {
                return NotFound($"Associated restaurant with ID {product.RestaurantId} was not found.");
            }

            if (!User.IsInRole("SystemAdmin") && restaurant.Owner?.Id != userIdGuid)
            {
                return Forbid();
            }

            var isDeleted = await _productService.DeleteAsync(id, cancellationToken);
            if (!isDeleted)
            {
                return NotFound($"Product with ID {id} was not found.");
            }

            return NoContent();
        }
    }
}