using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Application.Features.Users.Mapping;
using MyRestaurantApp.Application.Features.Users.IRepository;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Users.Services
{
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

            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }

            // 1. Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 2. Map Request -> Entity using Mapping Extension
            var user = request.ToEntity(passwordHash);

            await _userRepository.CreateAsync(user, cancellationToken);

            // 3. Return response with mapped UserResponse
            return new LoginResponse
            {
                Token = "fake-jwt-token",
                User = user.ToResponse()
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
                Token = "fake-jwt-token",
                User = user.ToResponse()
            };
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Map Request -> Entity using Mapping Extension
            var user = request.ToEntity(passwordHash, createdByUserId);

             user.CreatedAt = DateTime.UtcNow;
            user.CreatedBy = createdByUserId;
            await _userRepository.CreateAsync(user, cancellationToken);

            // Map Entity -> Response using Mapping Extension
            return user.ToResponse();
        }
     public async Task<bool> UpdateAsync(
    Guid userId,
    UpdateUserRequest request,
    Guid updatedByUserId,
    CancellationToken cancellationToken = default)
{
    var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

    if (user == null)
    {
        throw new Exception("User not found.");
    }

    request.ToEntity(user); //call the mapping extension method to update the existing user entity with the new values from the request

    user.UpdatedAt = DateTime.UtcNow;
    user.UpdatedBy = updatedByUserId;

    return await _userRepository.UpdateAsync(user, cancellationToken);
}

        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return users.Select(user => user.ToResponse());
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
        public async Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            return user?.ToResponse();
        }
    }
}