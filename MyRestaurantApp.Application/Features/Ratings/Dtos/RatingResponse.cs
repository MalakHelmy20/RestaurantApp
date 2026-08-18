using System;

namespace MyRestaurantApp.Application.Features.Ratings.Dtos
{
    public class RatingResponse
    {
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;

        public int RatingValue { get; set; }
        
    }
}