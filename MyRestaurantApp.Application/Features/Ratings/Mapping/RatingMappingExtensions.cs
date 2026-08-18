using System;
using System.Collections.Generic;
using System.Linq;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Ratings.Dtos;

namespace MyRestaurantApp.Application.Features.Ratings.Mapping
{
    public static class RatingMappingExtensions
    {
      
        public static Rating ToEntity(this CreateRatingRequest request, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(request);

            return new Rating
            {
                Id = Guid.NewGuid(),
                RestaurantId = request.RestaurantId,
                RatingValue = request.RatingValue,
                UserId = userId,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
        }

  
        public static void UpdateEntity(this Rating rating, UpdateRatingRequest request, Guid updatedByUserId)
        {
            ArgumentNullException.ThrowIfNull(rating);
            ArgumentNullException.ThrowIfNull(request);

            rating.RatingValue = request.RatingValue;
            rating.UpdatedBy = updatedByUserId;
            rating.UpdatedAt = DateTime.UtcNow;
        }

   
        public static RatingResponse ToResponse(this Rating rating)
        {
            ArgumentNullException.ThrowIfNull(rating);

            return new RatingResponse
            {
                Id = rating.Id,
                UserId = rating.UserId,
                UserName = rating.User != null 
                    ? $"{rating.User.FirstName} {rating.User.LastName}".Trim() 
                    : string.Empty,

                RestaurantId = rating.RestaurantId,
                RestaurantName = rating.Restaurant?.Name ?? string.Empty,

                RatingValue = rating.RatingValue,
                
            };
        }

        
        public static IEnumerable<RatingResponse> ToResponseList(this IEnumerable<Rating> ratings)
        {
            return ratings.Select(r => r.ToResponse());
        }
    }
}