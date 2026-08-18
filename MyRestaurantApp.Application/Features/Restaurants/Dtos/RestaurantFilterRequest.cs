

namespace MyRestaurantApp.Application.Features.Restaurants.Dtos
{   
public class RestaurantFilterRequest
{
   public string ? SearchTerm{ get; set; }
     public Guid? CategoryId { get; set; }

    public Guid?ProductId {get;set;}
   public int PageNumber { get; set; } = 1;
   public int PageSize { get; set; } = 10;

}
}