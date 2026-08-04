using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Users.Repositories{

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            _users.Add(user);
            return Task.FromResult(true);
        }

        public Task<User?>GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user=_users.SingleOrDefault(x=>x.Id==userId);
            return Task.FromResult(user);
        } 

        public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.AsEnumerable());
        }
       public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default) 
       {
            var userIndex=_users.FindIndex(x=>x.Id==user.Id);
            if (userIndex == -1)
            {
               return    Task.FromResult(false);
            }
            _users[userIndex]=user;
             return Task.FromResult(true);

        } 
      
     public Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var removedCount = _users.RemoveAll(x => x.Id == userId);
            var userRemoved = removedCount > 0;
            return Task.FromResult(userRemoved);
        }
       public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
      {
      var user = _users.FirstOrDefault(u => u.Email == email);
       return Task.FromResult(user);
     }
    
}
}


