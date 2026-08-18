using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Application.Features.Categories.Services;
using MyRestaurantApp.Application.Features.Categories.IRepository;
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
                
            };
        }

        public static Category ToEntity(this CreateCategoryRequest request)
        {
            return new Category
            {
                Name = request.Name,
                
            };
        }

        public static void UpdateEntity(
            this UpdateCategoryRequest request,
            Category category)
        {
            if (request.Name != null)
            {
                category.Name = request.Name;
            }

           
        }
    }
}