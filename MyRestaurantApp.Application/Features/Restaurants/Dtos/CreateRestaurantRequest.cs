
using System.ComponentModel.DataAnnotations;

using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Restaurants.Dtos
{
    public class CreateRestaurantRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Restaurant name is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Restaurant address is required.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Restaurant phone number is required.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Restaurant Description is required.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Open time is required.")]
        public TimeSpan? OpenTime { get; set; }

        [Required(ErrorMessage = "Close time is required.")]
        public TimeSpan? CloseTime { get; set; }

        //OPENTIME < CLOSETIME VALIDATION
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OpenTime.HasValue && CloseTime.HasValue && CloseTime == OpenTime)
            {
                yield return new ValidationResult(
                    "Close time cannot be the same as open time.",
                    new[] { nameof(CloseTime) }
                );
            }
        }

        //categories
        public List<Guid> CategoryIds { get; set; } = new();

        //owner
        [Required(ErrorMessage = "Owner ID is required.")]
        public Guid OwnerId { get; set; }

        //products
         public List<Guid> ProductIds { get; set; } = new();
    }

    }
