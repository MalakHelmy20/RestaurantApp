
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Application.Features.Products.Dtos;
namespace MyRestaurantApp.Application.Features.Restaurants.Dtos
{
    public class RestaurantResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public List<CategoryResponse> Categories { get; set; } = new();
          public List<ProductResponse> Products { get; set; } = new();
        public UserResponse Owner { get; set; } = null!;
    }
}