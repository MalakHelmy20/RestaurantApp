using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Restaurants.Dtos;

using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Features.Restaurants.IRepository{


    public interface IRestaurantRepository
    {
        Task<bool> CreateAsync(Restaurant restaurant, CancellationToken cancellationToken = default);
        Task<Restaurant?> GetByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Restaurant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Restaurant restaurant, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid restaurantId, CancellationToken cancellationToken = default);

      Task<IEnumerable<Restaurant>> GetFilteredAsync(RestaurantFilterRequest filter, CancellationToken cancellationToken = default);

        

    }

}
