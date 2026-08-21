using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Application.Features.Users.IRepository;
using MyRestaurantApp.Infrastructure;
namespace MyRestaurantApp.Infrastructure.Repository.UserRepo
{

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default)
{
    await _dbContext.Users.AddAsync(user, cancellationToken);
    return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
}

        public async Task<User?>GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Set<User>()
        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

         return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
           .AsNoTracking() 
           .ToListAsync(cancellationToken);
        }
       public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default) 
       {
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            if (existingUser is null)
            {
                return false;
            }
            _dbContext.Entry(existingUser).CurrentValues.SetValues(user);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;

        } 
      
     public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
           var user = await _dbContext.Users
        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

    if (user is null)
    {
        return false;
    }

    
    _dbContext.Users.Remove(user);
    
    return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
       public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
      {
      return await _dbContext.Users
        .AsNoTracking() 
        .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
     }
    
}
}