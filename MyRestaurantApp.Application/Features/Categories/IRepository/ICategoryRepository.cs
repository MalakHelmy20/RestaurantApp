using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Categories.Dtos;

using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Features.Categories.IRepository{

  public interface ICategoryRepository{
  Task <bool>CreateAsync(Category category , CancellationToken cancellationToken=default);
  Task<Category?>GetByIdAsync(Guid categoryId,CancellationToken cancellationToken=default);
  Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
   Task<bool> UpdateAsync( Category category ,CancellationToken cancellationToken = default);
   Task<bool> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
  }
}

