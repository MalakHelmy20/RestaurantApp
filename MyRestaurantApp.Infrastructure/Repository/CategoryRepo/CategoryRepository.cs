

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Categories.Dtos;
using MyRestaurantApp.Application.Features.Categories.IRepository;
namespace MyRestaurantApp.Infrastructure.Repository.CategoryRepo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly List <Category>_categories=new();

        public Task<bool>CreateAsync(Category category,CancellationToken cancellationToken = default)
        {
            _categories.Add(category);
            return Task.FromResult(true);

        }

        
       
        public Task<Category?>GetByIdAsync(Guid categoryId,CancellationToken cancellationToken = default)
        {
            var category=_categories.SingleOrDefault(x=>x.Id==categoryId);
            return Task.FromResult(category);
        }

        public Task<IEnumerable<Category>>GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_categories.AsEnumerable());
        }

          public Task<bool> UpdateAsync(Category category, CancellationToken cancellationToken = default) 
       {
            var categoryIndex=_categories.FindIndex(x=>x.Id==category.Id);
            if (categoryIndex == -1)
            {
               return    Task.FromResult(false);
            }
            _categories[categoryIndex]=category;
             return Task.FromResult(true);

        }

         public Task<bool> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var removedCount = _categories.RemoveAll(x => x.Id == categoryId);
            var categoryRemoved = removedCount > 0;
            return Task.FromResult(categoryRemoved);
        } 
    }
}
