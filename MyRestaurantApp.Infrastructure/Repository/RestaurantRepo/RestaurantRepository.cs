using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Restaurants.Dtos;
using MyRestaurantApp.Application.Features.Restaurants.IRepository;
namespace MyRestaurantApp.Infrastructure.Repository.RestaurantRepo
{

public class RestaurantRepository : IRestaurantRepository
{
   private readonly List<Restaurant> _restaurants = new();
   public Task<bool> CreateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            _restaurants.Add(restaurant);
            return Task.FromResult(true);
        }

        public Task<Restaurant?> GetByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var restaurant = _restaurants.SingleOrDefault(x => x.Id == restaurantId);
            return Task.FromResult(restaurant);
        }

        public Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_restaurants.AsEnumerable());
        }

        public Task<bool> UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            var restaurantIndex = _restaurants.FindIndex(x => x.Id == restaurant.Id);
            if (restaurantIndex == -1)
            {
                return Task.FromResult(false);
            }
            _restaurants[restaurantIndex] = restaurant;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var removedCount = _restaurants.RemoveAll(x => x.Id == restaurantId);
            var restaurantRemoved = removedCount > 0;
            return Task.FromResult(restaurantRemoved);
        }

        public Task<IEnumerable<Restaurant>> GetFilteredAsync(RestaurantFilterRequest filter, CancellationToken cancellationToken = default)
{
    var query = _restaurants.AsEnumerable();

    if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        query = query.Where(r => r.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));

    if (filter.CategoryId.HasValue)
        query = query.Where(r => r.RestaurantCategories.Any(rc => rc.CategoryId == filter.CategoryId.Value));

    query = query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize);

    return Task.FromResult(query);
}
    }

}
