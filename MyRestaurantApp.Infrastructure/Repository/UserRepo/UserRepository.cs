using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyRestaurantApp.Application.Features.Users.IRepository;
using MyRestaurantApp.Domain;

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

        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

      public async Task<IEnumerable<User>> GetAllAsync(string? role = null, CancellationToken cancellationToken = default)
{
    var query = _dbContext.Users.AsQueryable();

    if (!string.IsNullOrWhiteSpace(role))
    {
        query = query.Where(u => u.Role.ToString().ToLower() == role.Trim().ToLower());
        
    }

    return await query.ToListAsync(cancellationToken);
}
        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default) 
        {
          
            _dbContext.Users.Update(user);
            
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

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

       public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
{

    return await _dbContext.Users
        .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower(), cancellationToken);
}

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.AnyAsync(cancellationToken);
        }
    }
}
