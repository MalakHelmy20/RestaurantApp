using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Products.Dtos;
using MyRestaurantApp.Application.Features.Products.IRepository;
using MyRestaurantApp.Application.Features.Products.Mapping;

namespace MyRestaurantApp.Application.Features.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
        {
            var product = request.ToEntity();
            
            product.CreatedBy = createdByUserId;
            product.CreatedAt = DateTime.UtcNow;

            await _productRepository.CreateAsync(product, cancellationToken);
            

            return product.ToResponse();
        }

        public async Task<ProductResponse?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            return product?.ToResponse();
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);
            return products.ToResponseList();
        }

        public async Task<bool> UpdateAsync(Guid productId, UpdateProductRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null) return false;

            product.UpdateEntity(request);
            product.UpdatedBy = updatedByUserId;
            product.UpdatedAt = DateTime.UtcNow;

            var result = await _productRepository.UpdateAsync(product, cancellationToken);
            

            return result;
        }

        public async Task<bool> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var result = await _productRepository.DeleteAsync(productId, cancellationToken);
           
            

            return result;
        }

        public async Task<IEnumerable<ProductResponse>> GetFilteredAsync(ProductFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetFilteredAsync(filter, cancellationToken);
            return products.ToResponseList();
        }
    }
}