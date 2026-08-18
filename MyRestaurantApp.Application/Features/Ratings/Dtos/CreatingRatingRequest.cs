using System;
using System.ComponentModel.DataAnnotations;

namespace MyRestaurantApp.Application.Features.Ratings.Dtos
{
    public class CreateRatingRequest
    {
        [Required(ErrorMessage = "Restaurant ID is required.")]
        public Guid RestaurantId { get; set; }

        [Required(ErrorMessage = "Rating value is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; }
    }
}