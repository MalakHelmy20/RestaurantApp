using System.ComponentModel.DataAnnotations;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Orders.Dtos
{
    public class UpdateOrderRequest
    {
        [EnumDataType(typeof(statusTypes), ErrorMessage = "Invalid order status.")]
        public statusTypes? Status { get; set; }

        public List<CreateOrderItemsRequest>? OrderItems { get; set; }
    }
}
