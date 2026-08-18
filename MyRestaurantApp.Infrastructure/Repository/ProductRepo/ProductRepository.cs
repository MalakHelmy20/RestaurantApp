

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Products.Dtos;
using MyRestaurantApp.Application.Features.Products.IRepository;
namespace MyRestaurantApp.Infrastructure.Repository.ProductRepo
{
    public class ProductRepository : IProductRepository
    {
        private readonly List <Product>_products=new();

        public Task<bool>CreateAsync(Product product,CancellationToken cancellationToken = default)
        {
            _products.Add(product);
            return Task.FromResult(true);

        }

        
       
        public Task<Product?>GetByIdAsync(Guid productId,CancellationToken cancellationToken = default)
        {
            var product=_products.SingleOrDefault(x=>x.Id==productId);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
{
    var products = _products.Where(x => productIds.Contains(x.Id));
    return Task.FromResult(products);
}

        public Task<IEnumerable<Product>>GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_products.AsEnumerable());
        }

          public Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default) 
       {
            var productIndex=_products.FindIndex(x=>x.Id==product.Id);
            if (productIndex == -1)
            {
               return    Task.FromResult(false);
            }
            _products[productIndex]=product;
             return Task.FromResult(true);

        }

         public Task<bool> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var removedCount = _products.RemoveAll(x => x.Id == productId);
            var productRemoved = removedCount > 0;
            return Task.FromResult(productRemoved);
        } 

    public Task<IEnumerable<Product>> GetFilteredAsync(ProductFilterRequest filter, CancellationToken cancellationToken = default)
{
    var query = _products.AsEnumerable();


    if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
    {
        query = query.Where(p => p.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
    }

    var result = query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize);

    return Task.FromResult(result);
}
    }
}