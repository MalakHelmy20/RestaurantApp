
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Categories.Dtos;
namespace MyRestaurantApp.Application.Features.Categories.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
        Task<CategoryResponse?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid categoryId, UpdateCategoryRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);

    }
}