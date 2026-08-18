using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Products.Dtos;

using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Features.Products.IRepository{


    public interface IProductRepository
    {
        Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
       Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
      Task<IEnumerable<Product>> GetFilteredAsync(ProductFilterRequest filter, CancellationToken cancellationToken = default);

        

    }

}
