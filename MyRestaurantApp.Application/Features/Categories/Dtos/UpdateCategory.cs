using System;
using System.ComponentModel.DataAnnotations;

namespace MyRestaurantApp.Application.Features.Categories.Dtos
{
    public class UpdateCategoryRequest
    {
        public Guid? Id { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 characters.")]
        public string? Name { get; set; }

      
    }
}

      