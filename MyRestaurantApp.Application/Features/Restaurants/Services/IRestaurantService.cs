
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Restaurants.Dtos;
namespace MyRestaurantApp.Application.Features.Restaurants.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
        Task<RestaurantResponse?> GetByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<RestaurantResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid restaurantId, UpdateRestaurantRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid restaurantId, CancellationToken cancellationToken = default);


        Task<IEnumerable<RestaurantSummary>> GetFilteredAsync(RestaurantFilterRequest filter, CancellationToken cancellationToken = default);
    }
}