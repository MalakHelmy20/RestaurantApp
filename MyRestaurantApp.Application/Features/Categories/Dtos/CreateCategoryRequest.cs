using System.ComponentModel.DataAnnotations;

namespace MyRestaurantApp.Application.Features.Categories.Dtos
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Restaurant ID is required.")]
        public Guid RestaurantId { get; set; }

       
    }
}