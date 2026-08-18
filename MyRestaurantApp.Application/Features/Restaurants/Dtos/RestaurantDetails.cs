using System;
using System.Collections.Generic;
using System.Threading;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Application.Features.Products.Dtos;
namespace MyRestaurantApp.Application.Features.Restaurants.Dtos
{
public class RestaurantDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviewsCount { get; set; }
    
    
  public List<CategoryResponse> Categories { get; set; } = new();

  public List <ProductResponse>Products{get;set;}=new();
  public RestaurantOwner? Owner { get; set; }
  
}
}