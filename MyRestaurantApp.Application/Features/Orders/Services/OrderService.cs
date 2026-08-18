using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Orders.Dtos;
using MyRestaurantApp.Application.Features.Orders.IRepository;
using MyRestaurantApp.Application.Features.Orders.Mapping;
using MyRestaurantApp.Application.Features.Products.IRepository;
using MyRestaurantApp.Domain; 

namespace MyRestaurantApp.Application.Features.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository; 
        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
{
    IEnumerable<Guid> productIds = request.OrderItems.Select(i => i.ProductId);

  
    var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

    var order = request.ToEntity(createdByUserId, products.ToList());
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

            order.UpdateStatus(request);
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