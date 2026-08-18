using System.ComponentModel.DataAnnotations;

namespace MyRestaurantApp.Application.Features.Ratings.Dtos
{
    public class UpdateRatingRequest
    {
        [Required(ErrorMessage = "Rating value is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; }
    }
}