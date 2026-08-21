
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Application.Features.Categories.Dtos;
namespace MyRestaurantApp.Application.Features.Categories.Dtos
{
    public class CategoryResponse
    {

   
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    
  
        public int TotalProducts { get; set; }

        public Guid RestaurantId;
    }
}
