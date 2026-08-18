using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Ratings.Dtos;
using MyRestaurantApp.Application.Features.Ratings.Mapping;
using MyRestaurantApp.Application.Features.Ratings.IRepository;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Ratings.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<RatingResponse> CreateAsync(CreateRatingRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
        {
            var rating = request.ToEntity(createdByUserId);
            rating.CreatedAt = DateTime.UtcNow;
            rating.CreatedBy = createdByUserId;

            await _ratingRepository.CreateAsync(rating, cancellationToken);
            return rating.ToResponse();
        }

        public async Task<RatingResponse?> GetByIdAsync(Guid ratingId, CancellationToken cancellationToken = default)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId, cancellationToken);
            return rating?.ToResponse();
        }

        public async Task<IEnumerable<RatingResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var ratings = await _ratingRepository.GetAllAsync(cancellationToken);
            return ratings.Select(r => r.ToResponse());
        }

        public async Task<bool> UpdateAsync(Guid ratingId, UpdateRatingRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId, cancellationToken);
            if (rating == null)
            {
                throw new Exception("Rating not found.");
            }

            rating.UpdateEntity(request, updatedByUserId);

            return await _ratingRepository.UpdateAsync(rating, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid ratingId, CancellationToken cancellationToken = default)
        {
            var rating = await _ratingRepository.GetByIdAsync(ratingId, cancellationToken);
            if (rating == null)
            {
                throw new Exception("Rating not found.");
            }

            return await _ratingRepository.DeleteAsync(ratingId, cancellationToken);
        }

        public async Task<IEnumerable<RatingResponse>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var ratings = await _ratingRepository.GetByRestaurantIdAsync(restaurantId, cancellationToken);
            return ratings.Select(r => r.ToResponse());
        }
    }
}