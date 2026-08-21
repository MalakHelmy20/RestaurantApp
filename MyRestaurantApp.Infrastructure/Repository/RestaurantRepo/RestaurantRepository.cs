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
            await _dbContext.Restaurants.AddAsync(restaurant,cancellationToken);
           return await _dbContext.SaveChangesAsync(cancellationToken)>0;
        }

        public async Task<Restaurant?> GetByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _dbContext.Set<Restaurant>().SingleOrDefaultAsync(x => x.Id == restaurantId,cancellationToken);
            return restaurant;
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return
             await _dbContext.Restaurants
             .AsNoTracking().ToListAsync(cancellationToken);
        }

        public async  Task<bool> UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            var existingRestaurant= await _dbContext.Restaurants.FirstOrDefaultAsync(x => x.Id == restaurant.Id,cancellationToken);
            if (existingRestaurant is null)
            {
                return false;
            }
            _dbContext.Entry(existingRestaurant).CurrentValues.SetValues(restaurant);
           return await _dbContext.SaveChangesAsync(cancellationToken)>0;
        }

        public async Task<bool> DeleteAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
          var restaurant= await _dbContext.Restaurants.FirstOrDefaultAsync(x=>x.Id==restaurantId,cancellationToken);
          if(restaurant is null)
            {
                return false;
            }
            _dbContext.Restaurants.Remove(restaurant);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IEnumerable<Restaurant>> GetFilteredAsync(RestaurantFilterRequest filter, CancellationToken cancellationToken = default)
{
    var query = _dbContext.Restaurants.AsNoTracking().AsQueryable();

    if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        query = query.Where(r => r.Name.Contains(filter.SearchTerm));

    if (filter.CategoryId.HasValue)
        query = query.Where(r => r.RestaurantCategories.Any(rc => rc.CategoryId == filter.CategoryId.Value));

    query = query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize);

    return  await query.ToListAsync(cancellationToken);
}
    }

}
