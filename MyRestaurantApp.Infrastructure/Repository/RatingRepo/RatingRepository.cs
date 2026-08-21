using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Ratings.IRepository;

namespace MyRestaurantApp.Infrastructure.Repository.RatingRepo
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _dbContext;
        public RatingRepository(AppDbContext dbContext){
        
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAsync(Rating rating, CancellationToken cancellationToken = default)
        {
            await _dbContext.Ratings.AddAsync(rating,cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken)>0;
        }

        public async Task<Rating?> GetByIdAsync(Guid ratingId, CancellationToken cancellationToken = default)
        {
            var rating = await _dbContext.Set<Rating>().FirstOrDefaultAsync(x => x.Id == ratingId,cancellationToken);
            return rating;
        }

        public async Task<IEnumerable<Rating>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Ratings.AsNoTracking().ToListAsync(cancellationToken);
        }

       public async Task<IEnumerable<Rating>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
{
    return await _dbContext.Ratings
        .AsNoTracking()
        .Where(x => x.RestaurantId == restaurantId)
        .ToListAsync(cancellationToken);
}
        public async Task<bool> UpdateAsync(Rating rating, CancellationToken cancellationToken = default)
        {
            var existingRating= await _dbContext.Ratings.FirstOrDefaultAsync(x => x.Id == rating.Id,cancellationToken);
            if (existingRating is null)
            {
                return false;
            }

          _dbContext.Entry(existingRating).CurrentValues.SetValues(rating);
          return await _dbContext.SaveChangesAsync(cancellationToken)>0;

        }

        public async Task<bool> DeleteAsync(Guid ratingId, CancellationToken cancellationToken = default)
        {
            var rating = await _dbContext.Ratings.FirstOrDefaultAsync(x => x.Id == ratingId,cancellationToken);
            if(rating is null)
            {
                return false;
            }
            _dbContext.Ratings.Remove(rating);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
}