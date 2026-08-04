using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Users.Dtos;

using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Users.Repositories{

public interface IUserRepository
{
   Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
     Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

}
}


