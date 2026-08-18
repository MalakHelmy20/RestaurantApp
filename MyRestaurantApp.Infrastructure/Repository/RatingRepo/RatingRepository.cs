using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Ratings.IRepository;

namespace MyRestaurantApp.Infrastructure.Repository.RatingRepo
{
    public class RatingRepository : IRatingRepository
    {
        private readonly List<Rating> _ratings = new();

        public Task<bool> CreateAsync(Rating rating, CancellationToken cancellationToken = default)
        {
            _ratings.Add(rating);
            return Task.FromResult(true);
        }

        public Task<Rating?> GetByIdAsync(Guid ratingId, CancellationToken cancellationToken = default)
        {
            var rating = _ratings.FirstOrDefault(x => x.Id == ratingId);
            return Task.FromResult(rating);
        }

        public Task<IEnumerable<Rating>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_ratings.AsEnumerable());
        }

        public Task<IEnumerable<Rating>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
        {
            var restaurantRatings = _ratings.Where(x => x.RestaurantId == restaurantId).AsEnumerable();
            return Task.FromResult(restaurantRatings);
        }

        public Task<bool> UpdateAsync(Rating rating, CancellationToken cancellationToken = default)
        {
            var index = _ratings.FindIndex(x => x.Id == rating.Id);
            if (index == -1)
            {
                return Task.FromResult(false);
            }

            _ratings[index] = rating;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid ratingId, CancellationToken cancellationToken = default)
        {
            var removedCount = _ratings.RemoveAll(x => x.Id == ratingId);
            return Task.FromResult(removedCount > 0);
        }
    }
}