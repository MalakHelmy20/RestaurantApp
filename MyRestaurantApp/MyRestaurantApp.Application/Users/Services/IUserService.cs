using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyRestaurantApp.Application.Users.Dtos;

namespace MyRestaurantApp.Application.Users.Services;

public interface IUserService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> CreateAsync(CreateUserRequest request, Guid createdByUserId,CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync( Guid userId,UpdateUserRequest request, Guid updatedByUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId,CancellationToken cancellationToken = default);

}