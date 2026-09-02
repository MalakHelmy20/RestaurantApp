using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Infrastructure;
using MyRestaurantApp.Application.Features.Categories.IRepository;

namespace MyRestaurantApp.Infrastructure.Repository.CategoryRepo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _dbContext.Categories.AddAsync(category, cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories
                .Include(c => c.Products)
                .Include(c => c.RestaurantCategories)
                .FirstOrDefaultAsync(x => x.Id == categoryId, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories
                .Include(c => c.Products)
                .Include(c => c.RestaurantCategories)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default) 
        {
            if (_dbContext.Entry(category).State == EntityState.Detached)
            {
                var existingCategory = await _dbContext.Categories
                    .Include(c => c.RestaurantCategories)
                    .FirstOrDefaultAsync(x => x.Id == category.Id, cancellationToken);

                if (existingCategory is null)
                {
                    return false;
                }

                existingCategory.Name = category.Name;
                existingCategory.UpdatedAt = category.UpdatedAt;
                existingCategory.UpdatedBy = category.UpdatedBy;

                existingCategory.RestaurantCategories.Clear();
                foreach (var relation in category.RestaurantCategories)
                {
                    existingCategory.RestaurantCategories.Add(new RestaurantCategory
                    {
                        RestaurantId = relation.RestaurantId,
                        CategoryId = existingCategory.Id
                    });
                }
            }

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == categoryId, cancellationToken);

            if (category is null)
            {
                return false;
            }

            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        } 
    }
}
