using System;

namespace MyRestaurantApp.Application.Features.Orders.Dtos
{
    public class OrderItemsResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
       
    }
}