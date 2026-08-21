using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurantApp.Application.Features.Orders.Dtos;
using MyRestaurantApp.Application.Features.Orders.Services;
using MyRestaurantApp.Application.Features.Restaurants.Services;

namespace MyRestaurantApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IRestaurantService _restaurantService;

        public OrderController(IOrderService orderService, IRestaurantService restaurantService)
        {
            _orderService = orderService;
            _restaurantService = restaurantService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var order = await _orderService.CreateAsync(request, userIdGuid, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner,Customer")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetByIdAsync(id, cancellationToken);
            if (order == null)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            if (!User.IsInRole("SystemAdmin"))
            {
                if (User.IsInRole("Customer") && order.CustomerId != userIdGuid)
                {
                    return Forbid();
                }

                if (User.IsInRole("RestaurantOwner"))
                {
                    var restaurant = await _restaurantService.GetByIdAsync(order.RestaurantId, cancellationToken);
                    if (restaurant == null || restaurant.Owner?.Id != userIdGuid)
                    {
                        return Forbid();
                    }
                }
            }

            return Ok(order);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "SystemAdmin,RestaurantOwner")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var ownerGuid))
            {
                return Unauthorized();
            }

            var allOrders = await _orderService.GetAllAsync(cancellationToken);

            if (User.IsInRole("SystemAdmin"))
            {
                return Ok(allOrders);
            }

            var allRestaurants = await _restaurantService.GetAllAsync(cancellationToken);
            var ownerRestaurant = allRestaurants.FirstOrDefault(r => r.Owner != null && r.Owner.Id == ownerGuid);

            if (ownerRestaurant == null)
            {
                return NotFound("No restaurant found for the current owner.");
            }

            var ownerOrders = allOrders.Where(o => o.RestaurantId == ownerRestaurant.Id);
            return Ok(ownerOrders);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetByIdAsync(id, cancellationToken);
            if (order == null)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            if (order.CustomerId != userIdGuid)
            {
                return Forbid();
            }

            var isUpdated = await _orderService.UpdateAsync(id, request, userIdGuid, cancellationToken);
            if (!isUpdated)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            return Ok(isUpdated);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(currentUserId, out var userIdGuid))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetByIdAsync(id, cancellationToken);
            if (order == null)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            if (order.CustomerId != userIdGuid)
            {
                return Forbid();
            }

            var isDeleted = await _orderService.DeleteAsync(id, cancellationToken);
            if (!isDeleted)
            {
                return NotFound($"Order with ID {id} was not found.");
            }

            return Ok(isDeleted);
        }
    }
}