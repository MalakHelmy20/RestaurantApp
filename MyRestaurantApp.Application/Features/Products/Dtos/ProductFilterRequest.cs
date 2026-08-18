using System;

namespace MyRestaurantApp.Application.Features.Products.Dtos
{
    public class ProductFilterRequest
    {
        
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? RestaurantId { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }

    public int PageNumber { get; set; } = 1;
   public int PageSize { get; set; } = 10;
    }
}