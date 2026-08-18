using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Features.Orders.Dtos
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;   
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;   

        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty; 

        public List<OrderItemsResponse> OrderItems { get; set; } = new();
    }

}