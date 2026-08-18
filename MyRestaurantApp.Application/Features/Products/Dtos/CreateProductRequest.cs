using MyRestaurantApp.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyRestaurantApp.Application.Features.Products.Dtos
{
    
public class CreateProductRequest{
[Required(ErrorMessage = "Product name is required.")]
[StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
public string Name {get;set;}=string.Empty;


[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
public string? Description { get; set; }

[Required(ErrorMessage = "Price is required.")]
[Range(0.01, 10000.00, ErrorMessage = "Price must be greater than zero.")]
public decimal Price { get; set; }


public bool IsAvailable { get; set; } = true;



//fks
[Required(ErrorMessage = "Category ID is required.")]
 public Guid CategoryId { get; set; }

[Required(ErrorMessage = "Restaurant ID is required.")]
public Guid RestaurantId { get; set; }



}
}