using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Ratings.Dtos;

using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Features.Ratings.IRepository{


    public interface IRatingRepository
    {
        Task<bool> CreateAsync(Rating rating, CancellationToken cancellationToken = default);
        Task<Rating?> GetByIdAsync(Guid ratingId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Rating>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Rating rating, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid ratingId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Rating>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
        Task<Rating?> GetByUserAndRestaurantAsync(Guid userId, Guid restaurantId, CancellationToken cancellationToken = default);
    }

}