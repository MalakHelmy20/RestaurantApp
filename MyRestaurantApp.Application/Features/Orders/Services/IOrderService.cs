
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Orders.Dtos;
namespace MyRestaurantApp.Application.Features.Orders.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateAsync(CreateOrderRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
        Task<OrderResponse?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid orderId, UpdateOrderRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default);



    }
}