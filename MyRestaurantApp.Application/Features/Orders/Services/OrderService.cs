using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Orders.Dtos;
using MyRestaurantApp.Application.Features.Orders.IRepository;
using MyRestaurantApp.Application.Features.Orders.Mapping;
using MyRestaurantApp.Application.Features.Products.IRepository;
using MyRestaurantApp.Application.Features.Restaurants.IRepository;
using MyRestaurantApp.Domain; 

namespace MyRestaurantApp.Application.Features.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IRestaurantRepository _restaurantRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IRestaurantRepository restaurantRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _restaurantRepository = restaurantRepository;
        }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
{
    var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId, cancellationToken);
    if (restaurant == null)
    {
        throw new KeyNotFoundException("Restaurant was not found.");
    }

    IEnumerable<Guid> productIds = request.OrderItems.Select(i => i.ProductId);
    var products = (await _productRepository.GetByIdsAsync(productIds, cancellationToken)).ToList();

    foreach (var item in request.OrderItems)
    {
        var product = products.FirstOrDefault(p => p.Id == item.ProductId);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product {item.ProductId} was not found.");
        }

        if (!product.IsAvailable)
        {
            throw new InvalidOperationException($"Product '{product.Name}' is not available.");
        }

        if (product.RestaurantId != request.RestaurantId)
        {
            throw new InvalidOperationException($"Product '{product.Name}' does not belong to this restaurant.");
        }
    }

    var order = request.ToEntity(createdByUserId, products);
    order.CreatedBy = createdByUserId;
    order.CreatedAt = DateTime.UtcNow;

    await _orderRepository.CreateAsync(order, cancellationToken);

    return order.ToResponse();
}
        public async Task<OrderResponse?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            return order?.ToResponse();
        }

        public async Task<IEnumerable<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            return orders.ToResponseList();
        }

        public async Task<bool> UpdateAsync(Guid orderId, UpdateOrderRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
            if (order == null) return false;

            List<Product>? products = null;
            if (request.OrderItems != null)
            {
                if (request.OrderItems.Count == 0)
                {
                    throw new InvalidOperationException("Order must contain at least one item.");
                }

                var productIds = request.OrderItems.Select(i => i.ProductId);
                products = (await _productRepository.GetByIdsAsync(productIds, cancellationToken)).ToList();

                foreach (var item in request.OrderItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Product {item.ProductId} was not found.");
                    }

                    if (!product.IsAvailable)
                    {
                        throw new InvalidOperationException($"Product '{product.Name}' is not available.");
                    }

                    if (product.RestaurantId != order.RestaurantId)
                    {
                        throw new InvalidOperationException($"Product '{product.Name}' does not belong to this restaurant.");
                    }
                }
            }

            order.ApplyUpdate(request, products);
            order.UpdatedBy = updatedByUserId;
            order.UpdatedAt = DateTime.UtcNow;

            return await _orderRepository.UpdateAsync(order, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var result = await _orderRepository.DeleteAsync(orderId, cancellationToken);
            return result;
        }
    }
}
