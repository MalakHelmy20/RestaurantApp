
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using MyRestaurantApp.Domain;
namespace MyRestaurantApp.Application.Features.Orders.Dtos{



public class CreateOrderItemsRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
    public int Quantity { get; set; }
}
}