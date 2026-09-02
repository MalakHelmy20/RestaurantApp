
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Ratings.Dtos;
namespace MyRestaurantApp.Application.Features.Ratings.Services
{
    public interface IRatingService
    {
        Task<RatingResponse> CreateAsync(CreateRatingRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
        Task<RatingResponse?> GetByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<RatingResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid restaurantId, UpdateRatingRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<IEnumerable<RatingResponse>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    }
}