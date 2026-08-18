using System;

namespace MyRestaurantApp.Application.Features.Products.Dtos
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        
        // Category and restaurant
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid RestaurantId { get; set; }
    }
}