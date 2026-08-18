using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Orders.Dtos;

using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Features.Orders.IRepository{


    public interface IOrderRepository
    {
        Task<bool> CreateAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default);



        

    }

}