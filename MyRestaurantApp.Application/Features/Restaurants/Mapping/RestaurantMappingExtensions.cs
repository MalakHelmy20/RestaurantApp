
using System;
using MyRestaurantApp.Application.Features.Restaurants.Dtos;
using MyRestaurantApp.Application.Features.Users.Mapping;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Application.Features.Categories.Mapping;
using MyRestaurantApp.Application.Features.Products.Dtos;
using MyRestaurantApp.Application.Features.Products.Mapping;


namespace MyRestaurantApp.Application.Features.Restaurants.Mapping
{

public static class RestaurantMappingExtensions
    {
        // 1. Convert Restaurant Entity to RestaurantResponse (Response Mapping)
        public static RestaurantResponse ToResponse(this Restaurant restaurant)
        {
            return new RestaurantResponse
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Address = restaurant.Address,
                OpenTime = restaurant.OpenTime,
                CloseTime = restaurant.CloseTime,

              Categories = restaurant.RestaurantCategories?
            .Where(rc => rc.Category != null) 
            .Select(rc => rc.Category!.ToResponse())
            .ToList() ?? new List<CategoryResponse>(),

               Products = restaurant.Products?
                    .Select(p => p.ToResponse())
                    .ToList() ?? new List<ProductResponse>(),


                Owner = restaurant.Owner?.ToResponse() ?? null!
              
            };
        }

       public static Restaurant ToEntity(this CreateRestaurantRequest request, Guid ownerId)
{
    return new Restaurant
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Description = request.Description,
        Address = request.Address,
        Phone = request.Phone,
        OpenTime = request.OpenTime!.Value,
        CloseTime = request.CloseTime!.Value,
        OwnerId = ownerId,
        RestaurantCategories = request.CategoryIds
            .Select(catId => new RestaurantCategory { CategoryId = catId })
            .ToList()
        
    };
}
   public static Restaurant ToEntity(
    this UpdateRestaurantRequest request,
    Restaurant existingRestaurant)
{
    if (request.Name != null)
        existingRestaurant.Name = request.Name;

    if (request.Description != null)
        existingRestaurant.Description = request.Description;

    if (request.Address != null)
        existingRestaurant.Address = request.Address;

    if (request.OpenTime.HasValue)
        existingRestaurant.OpenTime = request.OpenTime.Value;

    if (request.CloseTime.HasValue)
        existingRestaurant.CloseTime = request.CloseTime.Value;

    if (request.Phone != null)
        existingRestaurant.Phone = request.Phone;

   

    if (request.CategoryIds != null)
    {
        existingRestaurant.RestaurantCategories = request.CategoryIds
            .Select(catId => new RestaurantCategory { CategoryId = catId })
            .ToList();
    }

    if (request.OwnerId.HasValue)
        existingRestaurant.OwnerId = request.OwnerId.Value;

    return existingRestaurant;
}
 public static RestaurantSummary ToSummary(this Restaurant restaurant)
{
    return new RestaurantSummary
    {
        Id = restaurant.Id,
        Name = restaurant.Name,
        Address = restaurant.Address,
        AverageRating = restaurant.Ratings.Any() ? restaurant.Ratings.Average(r => r.RatingValue) : 0.0,

      
    };
}
//  UserMappingExtensions.cs or RestaurantMappingExtensions.cs
public static RestaurantOwner ToRestaurantOwner(this User user)
{
    return new RestaurantOwner
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone
    };
}
public static RestaurantDetails ToDetails(this Restaurant restaurant)
        {
            return new RestaurantDetails
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                OpenTime = restaurant.OpenTime,
                CloseTime = restaurant.CloseTime,
                AverageRating = restaurant.Ratings.Any() ? restaurant.Ratings.Average(r => r.RatingValue) : 0.0,
                TotalReviewsCount = restaurant.Ratings.Count(),

                Products = restaurant.Products?
                    .Select(p => p.ToResponse())
                  .ToList() ?? new List<ProductResponse>(),
                
                  Owner = restaurant.Owner?.ToRestaurantOwner()
            };
        }

        
    }


}
