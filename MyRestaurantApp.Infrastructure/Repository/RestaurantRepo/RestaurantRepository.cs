using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Infrastructure;
using MyRestaurantApp.Application.Features.Restaurants.Dtos;
using MyRestaurantApp.Application.Features.Restaurants.IRepository;

namespace MyRestaurantApp.Infrastructure.Repository.RestaurantRepo
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly AppDbContext _dbContext;

        public RestaurantRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            await _dbContext.Restaurants.AddAsync(restaurant, cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<Restaurant?> GetByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            return await WithDetails(_dbContext.Restaurants)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == restaurantId && !x.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await WithDetails(_dbContext.Restaurants.Where(r => !r.IsDeleted))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            var existingRestaurant = await _dbContext.Restaurants
                .Include(r => r.RestaurantCategories)
                .FirstOrDefaultAsync(x => x.Id == restaurant.Id, cancellationToken);
            if (existingRestaurant is null)
            {
                return false;
            }

            _dbContext.Entry(existingRestaurant).CurrentValues.SetValues(restaurant);

            existingRestaurant.RestaurantCategories.Clear();
            foreach (var relation in restaurant.RestaurantCategories ?? Enumerable.Empty<RestaurantCategory>())
            {
                existingRestaurant.RestaurantCategories.Add(new RestaurantCategory
                {
                    RestaurantId = existingRestaurant.Id,
                    CategoryId = relation.CategoryId
                });
            }

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(Guid restaurantId, CancellationToken cancellationToken)
        {
            var restaurant = await _dbContext.Restaurants.FirstOrDefaultAsync(r => r.Id == restaurantId, cancellationToken);
            if (restaurant == null) return false;

            restaurant.IsDeleted = true;
            restaurant.DeletedAt = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IEnumerable<Restaurant>> GetFilteredAsync(RestaurantFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Restaurants
                .Where(r => !r.IsDeleted)
                .AsNoTracking()
                .Include(r => r.Ratings)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(r => r.Name.Contains(filter.SearchTerm));

            if (filter.CategoryId.HasValue)
                query = query.Where(r => r.RestaurantCategories.Any(rc => rc.CategoryId == filter.CategoryId.Value));

            if (filter.ProductId.HasValue)
                query = query.Where(r => r.Products.Any(p => p.Id == filter.ProductId.Value));

            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, 100);

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync(cancellationToken);
        }

        private static IQueryable<Restaurant> WithDetails(IQueryable<Restaurant> query)
        {
            return query
                .AsSplitQuery()
                .Include(r => r.RestaurantCategories)
                    .ThenInclude(rc => rc.Category)
                .Include(r => r.Products)
                .Include(r => r.Owner)
                .Include(r => r.Ratings);
        }
    }
}