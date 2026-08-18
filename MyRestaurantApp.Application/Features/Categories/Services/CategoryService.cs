using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Application.Features.Categories.IRepository;
using MyRestaurantApp.Application.Features.Categories.Mapping;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Categories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponse> CreateAsync(
            CreateCategoryRequest request,
            Guid createdByUserId,
            CancellationToken cancellationToken = default)
        {
            var category = request.ToEntity();

            category.CreatedBy = createdByUserId;

            await _categoryRepository.CreateAsync(
                category,
                cancellationToken);

            return category.ToResponse();
        }

        public async Task<CategoryResponse?> GetByIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(
                categoryId,
                cancellationToken);

            if (category == null)
                return null;

            return category.ToResponse();
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(
                cancellationToken);

            return categories.Select (c=>c.ToResponse());
             
        }

        public async Task<bool> UpdateAsync(
            Guid categoryId,
            UpdateCategoryRequest request,
            Guid updatedByUserId,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(
                categoryId,
                cancellationToken);

            if (category == null)
                return false;

            request.UpdateEntity(category);

            category.UpdatedBy= updatedByUserId;
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(
                category,
                cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            Guid categoryId,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(
                categoryId,
                cancellationToken);

            if (category == null)
                return false;

            await _categoryRepository.DeleteAsync(
                categoryId,
                cancellationToken);

            return true;
        }
    }
}
