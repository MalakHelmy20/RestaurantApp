using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Application.Features.Users.IRepository;
using MyRestaurantApp.Application.Features.Users.Mapping;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Password != request.confirmPassword)
            {
                throw new InvalidOperationException("Password and confirmation do not match.");
            }

            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = request.ToEntity(passwordHash);

            if (!await _userRepository.AnyAsync(cancellationToken))
            {
                user.Role = UserRole.SystemAdmin;
            }

            await _userRepository.CreateAsync(user, cancellationToken);
            var token = GenerateJwtToken(user);

            // 3. Return response with mapped UserResponse
            return new LoginResponse
            {
                Token = token,
                User = user.ToResponse()
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var cleanEmail = request.Email.Trim().ToLower();
            var user = await _userRepository.GetByEmailAsync(cleanEmail, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                User = user.ToResponse()
            };
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request, Guid createdByUserId, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
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
                throw new KeyNotFoundException("User not found.");
            }

            request.ToEntity(user);

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = updatedByUserId;

            return await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync(string? role = null, CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllAsync(role, cancellationToken);
            return users.Select(u => u.ToResponse());
        }

        public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return await _userRepository.DeleteAsync(userId, cancellationToken);
        }

        public async Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            return user?.ToResponse();
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}