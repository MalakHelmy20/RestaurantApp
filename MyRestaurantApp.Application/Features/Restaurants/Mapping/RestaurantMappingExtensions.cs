
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
    var activeProducts = GetActiveProducts(restaurant);
    var activeRatings = GetActiveRatings(restaurant);

    return new RestaurantResponse
    {
        Id = restaurant.Id,
        Name = restaurant.Name,
        Description = restaurant.Description,
        Address = restaurant.Address,
        Phone = restaurant.Phone,
        OpenTime = restaurant.OpenTime,
        CloseTime = restaurant.CloseTime,
        AverageRating = CalculateAverageRating(activeRatings),
        TotalRatingsCount = activeRatings.Count,

        Categories = restaurant.RestaurantCategories?
            .Where(rc => rc.Category != null && !rc.Category.IsDeleted)
            .Select(rc => MapCategory(rc.Category!, restaurant.Id, activeProducts))
            .ToList() ?? new List<CategoryResponse>(),

        Products = activeProducts
            .Select(p => p.ToResponse())
            .ToList(),

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
            .Select(catId => new RestaurantCategory { RestaurantId = existingRestaurant.Id, CategoryId = catId })
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
        AverageRating = CalculateAverageRating(GetActiveRatings(restaurant)),
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
            var activeProducts = GetActiveProducts(restaurant);
            var activeRatings = GetActiveRatings(restaurant);

            return new RestaurantDetails
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                OpenTime = restaurant.OpenTime,
                CloseTime = restaurant.CloseTime,
                AverageRating = CalculateAverageRating(activeRatings),
                TotalReviewsCount = activeRatings.Count,
                Categories = restaurant.RestaurantCategories?
                    .Where(rc => rc.Category != null && !rc.Category.IsDeleted)
                    .Select(rc => MapCategory(rc.Category!, restaurant.Id, activeProducts))
                    .ToList() ?? new List<CategoryResponse>(),
                Products = activeProducts
                    .Select(p => p.ToResponse())
                    .ToList(),
                Owner = restaurant.Owner?.ToRestaurantOwner()
            };
        }

        private static List<Product> GetActiveProducts(Restaurant restaurant)
        {
            return restaurant.Products?
                .Where(p => !p.IsDeleted)
                .ToList() ?? new List<Product>();
        }

        private static List<Rating> GetActiveRatings(Restaurant restaurant)
        {
            return restaurant.Ratings?
                .Where(r => !r.IsDeleted)
                .ToList() ?? new List<Rating>();
        }

        private static double CalculateAverageRating(IReadOnlyCollection<Rating> ratings)
        {
            return ratings.Count > 0
                ? Math.Round(ratings.Average(r => (double)r.RatingValue), 1)
                : 0.0;
        }

        private static CategoryResponse MapCategory(Category category, Guid restaurantId, IReadOnlyCollection<Product> restaurantProducts)
        {
            var response = category.ToResponse();
            response.RestaurantId = restaurantId;
            response.TotalProducts = restaurantProducts.Count(p => p.CategoryId == category.Id);

            if (restaurantProducts.Count == 0)
            {
                response.TotalProducts = category.Products?.Count(p => !p.IsDeleted) ?? 0;
            }

            return response;
        }
    }


}
