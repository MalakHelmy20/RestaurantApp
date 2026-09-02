
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Restaurants.Dtos;
using MyRestaurantApp.Application.Features.Restaurants.Mapping;
using MyRestaurantApp.Application.Features.Restaurants.IRepository;
using MyRestaurantApp.Application.Features.Users.IRepository;
using MyRestaurantApp.Domain;

namespace  MyRestaurantApp.Application.Features.Restaurants.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository, IUserRepository userRepository)
        {
            _restaurantRepository = restaurantRepository;
            _userRepository = userRepository;
        }

        public async Task<RestaurantResponse> CreateAsync(CreateRestaurantRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
        {
            var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
            if (owner == null)
            {
                throw new KeyNotFoundException("Owner was not found.");
            }

            if (owner.Role != UserRole.RestaurantOwner && owner.Role != UserRole.SystemAdmin)
            {
                throw new InvalidOperationException("The specified user cannot own a restaurant.");
            }

            var restaurant = request.ToEntity(request.OwnerId);
            restaurant.CreatedAt = DateTime.UtcNow;
            restaurant.CreatedBy = createdByUserId;
            await _restaurantRepository.CreateAsync(restaurant, cancellationToken);
            return restaurant.ToResponse();
        }

        public async Task<RestaurantResponse?> GetByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
            return restaurant?.ToResponse();
        }

        public async Task<IEnumerable<RestaurantResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var restaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
            return restaurants.Select(r => r.ToResponse());
        }

        public async Task<bool> UpdateAsync(Guid restaurantId, UpdateRestaurantRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
            if (restaurant == null)
            {
                throw new KeyNotFoundException("Restaurant not found.");
            }

            request.ToEntity(restaurant);

            restaurant.UpdatedAt = DateTime.UtcNow;
            restaurant.UpdatedBy = updatedByUserId;

            return await _restaurantRepository.UpdateAsync(restaurant, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
            if (restaurant == null)
            {
                throw new KeyNotFoundException("Restaurant not found.");
            }

            return await _restaurantRepository.DeleteAsync(restaurantId, cancellationToken);
        }

        public async Task<IEnumerable<RestaurantSummary>> GetFilteredAsync(RestaurantFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var restaurants = await _restaurantRepository.GetFilteredAsync(filter, cancellationToken);
            return restaurants.Select(r => r.ToSummary());
        }
    }
}
