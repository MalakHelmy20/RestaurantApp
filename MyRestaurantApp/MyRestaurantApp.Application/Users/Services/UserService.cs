using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Users.Dtos;
using MyRestaurantApp.Application.Users.Repositories;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Users.Services{

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

  public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
{
    if (request.Password != request.confirmPassword)
    {
        throw new Exception("Password and confirmation do not match.");
    }

    var existing_user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
    if (existing_user != null)
    {
        throw new Exception("User with this email already exists.");
    }

    var user = new User
    {
        Id = Guid.NewGuid(),
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        Phone = request.Phone,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password), // Hash 
        Role = UserRole.Customer
    };

    await _userRepository.CreateAsync(user, cancellationToken);

    return new LoginResponse
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Role = user.Role.ToString(),
        Token = "fake-jwt-token"
    };
}
 public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
{
    var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
    {
        throw new Exception("Invalid credentials.");
    }

    return new LoginResponse
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Role = user.Role.ToString(),
        Token = "fake-jwt-token"
    };
}
  public async Task<UserResponse> CreateAsync(CreateUserRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
{
    var user = new User
    {

        Id = Guid.NewGuid(),
        Email = request.Email,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Phone = request.Phone,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        CreatedBy = createdByUserId, //passing it through the controller
        CreatedAt = DateTime.UtcNow,
        Role = request.Role

    };

    await _userRepository.CreateAsync(user, cancellationToken);

    return new UserResponse
    {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Phone = user.Phone,
        Role = user.Role.ToString()
    };
}
public async Task<bool> UpdateAsync(Guid userId,UpdateUserRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default)
{
    var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
    if (user == null)
    {
        throw new Exception("User not found.");
    }

    user.FirstName = request.FirstName;
    user.LastName = request.LastName;
    user.Email = request.Email;
    user.Phone = request.Phone;
    user.UpdatedAt = DateTime.UtcNow;
    user.UpdatedBy = updatedByUserId; // passing it through the controller

    return await _userRepository.UpdateAsync(user, cancellationToken);
}


public  async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return users.Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString()
            });
        }

    
public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return await _userRepository.DeleteAsync(userId, cancellationToken);
    }
}
}




