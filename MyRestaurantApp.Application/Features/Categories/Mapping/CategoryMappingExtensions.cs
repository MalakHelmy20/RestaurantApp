using System.Linq;
using System.Collections.Generic;
using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Categories.Mapping
{
    public static class CategoryMappingExtensions
    {
        public static CategoryResponse ToResponse(this Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                TotalProducts = category.Products?.Count ?? 0,
                
                RestaurantId = category.RestaurantCategories?.FirstOrDefault()?.RestaurantId ?? Guid.Empty
            };
        }

        public static Category ToEntity(this CreateCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name
            };

           
            category.RestaurantCategories.Add(new RestaurantCategory
            {
                RestaurantId = request.RestaurantId,
                CategoryId = category.Id
            });

            return category;
        }

       public static void UpdateEntity(this UpdateCategoryRequest request, Category category)
{
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            category.Name = request.Name;
        }

        if (request.RestaurantId != Guid.Empty)
        {
            category.RestaurantCategories ??= new List<RestaurantCategory>();

            var relationsToRemove = category.RestaurantCategories
                .Where(rc => rc.RestaurantId != request.RestaurantId)
                .ToList();

            foreach (var relation in relationsToRemove)
            {
                category.RestaurantCategories.Remove(relation);
            }

            if (!category.RestaurantCategories.Any(rc => rc.RestaurantId == request.RestaurantId))
            {
                category.RestaurantCategories.Add(new RestaurantCategory
                {
                    RestaurantId = request.RestaurantId,
                    CategoryId = category.Id
                });
            }
        }
}
    }
}