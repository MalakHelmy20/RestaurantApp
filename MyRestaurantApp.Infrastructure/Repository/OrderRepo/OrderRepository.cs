

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Orders.Dtos;
using MyRestaurantApp.Application.Features.Orders.IRepository;
namespace MyRestaurantApp.Infrastructure.Repository.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly List <Order>_orders=new();

        public Task<bool>CreateAsync(Order order,CancellationToken cancellationToken = default)
        {
            _orders.Add(order);
            return Task.FromResult(true);

        }

        
       
        public Task<Order?>GetByIdAsync(Guid orderId,CancellationToken cancellationToken = default)
        {
            var order=_orders.SingleOrDefault(x=>x.Id==orderId);
            return Task.FromResult(order);
        }

        public Task<IEnumerable<Order>>GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_orders.AsEnumerable());
        }

          public Task<bool> UpdateAsync(Order order, CancellationToken cancellationToken = default) 
       {
            var orderIndex=_orders.FindIndex(x=>x.Id==order.Id);
            if (orderIndex == -1)
            {
               return    Task.FromResult(false);
            }
            _orders[orderIndex]=order;
             return Task.FromResult(true);

        }

         public Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var removedCount = _orders.RemoveAll(x => x.Id == orderId);
            var orderRemoved = removedCount > 0;
            return Task.FromResult(orderRemoved);
        } 
    }
}
