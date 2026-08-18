using MyRestaurantApp.Domain;
using System;
using System.ComponentModel.DataAnnotations;


namespace MyRestaurantApp.Application.Features.Orders.Dtos
{
    public class CreateOrderRequest
    {
       [Required(ErrorMessage = "Restaurant ID is required.")]
        public Guid RestaurantId { get; set; }

        [Required(ErrorMessage = "Order must contain at least one item.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<CreateOrderItemsRequest> OrderItems { get; set; } = new();

}
}