using System;
using System.Collections.Generic;
using System.Linq;
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
            return await WithDetails(_dbContext.Orders)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await WithDetails(_dbContext.Orders)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Order order, CancellationToken cancellationToken = default) 
        {
            //make it tracked
            var existingOrder = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(x => x.Id == order.Id, cancellationToken);

            if (existingOrder is null)
            {
                return false;
            }

         
            existingOrder.status = order.status;
            existingOrder.TotalPrice = order.TotalPrice;
            existingOrder.UpdatedAt = order.UpdatedAt;
            existingOrder.UpdatedBy = order.UpdatedBy;

            // remove the olditems from database
            if (existingOrder.OrderItems.Any())
            {
                _dbContext.OrderItems.RemoveRange(existingOrder.OrderItems);
            }

           
            foreach (var item in order.OrderItems)
            {
                _dbContext.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(), 
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    OrderId = existingOrder.Id
                });
            }

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

            order.IsDeleted = true;
            order.DeletedAt = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        private static IQueryable<Order> WithDetails(IQueryable<Order> query)
        {
            return query
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .Include(o => o.Restaurant);
        }
    }
}