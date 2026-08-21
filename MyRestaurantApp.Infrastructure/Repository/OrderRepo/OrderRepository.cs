using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Infrastructure;
using MyRestaurantApp.Application.Features.Orders.IRepository;

namespace MyRestaurantApp.Infrastructure.Repository.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Order order, CancellationToken cancellationToken = default) 
        {
            var existingOrder = await _dbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellationToken);

            if (existingOrder is null)
            {
                return false;
            }

            _dbContext.Entry(existingOrder).CurrentValues.SetValues(order);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

            if (order is null)
            {
                return false;
            }

            _dbContext.Orders.Remove(order);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        } 
    }
}