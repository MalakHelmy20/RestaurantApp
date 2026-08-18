using System;
using System.ComponentModel.DataAnnotations;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Restaurants.Dtos
{
    public class UpdateRestaurantRequest
    {
        public Guid? Id { get; set; }

        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Restaurant name must be between 2 and 100 characters.")]
        public string? Name { get; set; }

        [StringLength(200, MinimumLength = 5,
            ErrorMessage = "Restaurant address must be between 5 and 200 characters.")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }

     
        [StringLength(200, MinimumLength = 10,
            ErrorMessage = "Restaurant description must be between 10 and 200 characters.")]
        public string? Description { get; set; }

        public TimeSpan? OpenTime { get; set; }

        public TimeSpan? CloseTime { get; set; }

        public List<Guid>? CategoryIds { get; set; }

         public List<Guid>? productIds { get; set; }

        public Guid? OwnerId { get; set; }
    }
}

