
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Application.Features.Products.Dtos;
using MyRestaurantApp.Application.Features.Products.IRepository;
namespace MyRestaurantApp.Infrastructure.Repository.ProductRepo
{
    public class ProductRepository : IProductRepository
    {
       private readonly AppDbContext _dbContext;
       public ProductRepository (AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool>CreateAsync(Product product,CancellationToken cancellationToken = default)
        {
            await _dbContext.Products.AddAsync(product,cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken)>0;

        }

        public async Task<Product?>GetByIdAsync(Guid productId,CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);
        }

  public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
{
    return await _dbContext.Products
        .AsNoTracking()
        .Where(x => productIds.Contains(x.Id))
        .ToListAsync(cancellationToken);
}
        public async Task<IEnumerable<Product>>GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

          public  async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default) 
       {
            var existingProduct=await _dbContext.Products.FirstOrDefaultAsync(x=>x.Id==product.Id,cancellationToken);
            if (existingProduct is null)
            {
               return    false;
            }
          _dbContext.Entry(existingProduct).CurrentValues.SetValues(product);
         return await _dbContext.SaveChangesAsync(cancellationToken)>0;

        }

         public async Task<bool> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        {
           var product = await _dbContext.Products.FirstOrDefaultAsync(p=>p.Id==productId,cancellationToken);
           if (product is null)
            {
                return false;
            }
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync(cancellationToken)>0;
        } 

    public async Task<IEnumerable<Product>> GetFilteredAsync(ProductFilterRequest filter, CancellationToken cancellationToken = default)
{
    var query = _dbContext.Products
        .Include(p => p.Category)
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
    {
        query = query.Where(p => p.Name.Contains(filter.SearchTerm));
    }

    if (filter.CategoryId.HasValue)
    {
        query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
    }

    if (filter.RestaurantId.HasValue)
    {
        query = query.Where(p => p.RestaurantId == filter.RestaurantId.Value);
    }

    if (filter.Price.HasValue)
    {
        query = query.Where(p => p.Price <= filter.Price.Value);
    }

    if (filter.IsAvailable.HasValue)
    {
        query = query.Where(p => p.IsAvailable == filter.IsAvailable.Value);
    }

    var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
    var pageSize = filter.PageSize < 1 ? 10 : Math.Min(filter.PageSize, 100);

    return await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
}
    }
}
