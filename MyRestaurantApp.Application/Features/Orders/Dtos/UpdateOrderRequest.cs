using System.ComponentModel.DataAnnotations;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Orders.Dtos
{
    public class UpdateOrderRequest
    {
        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(statusTypes), ErrorMessage = "Invalid order status.")]
        public statusTypes Status { get; set; }
    }
}