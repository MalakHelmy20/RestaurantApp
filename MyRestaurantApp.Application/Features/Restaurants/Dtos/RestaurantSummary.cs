namespace MyRestaurantApp.Application.Features.Restaurants.Dtos
{
    public class RestaurantSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        
    }
}