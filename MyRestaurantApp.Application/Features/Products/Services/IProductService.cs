
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Products.Dtos;
namespace MyRestaurantApp.Application.Features.Products.Services
{
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest request, Guid createdByUserId, CancellationToken cancellationToken = default);
        Task<ProductResponse?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid productId, UpdateProductRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid productId, CancellationToken cancellationToken = default);


        Task<IEnumerable<ProductResponse>> GetFilteredAsync(ProductFilterRequest filter, CancellationToken cancellationToken = default);
    }
}