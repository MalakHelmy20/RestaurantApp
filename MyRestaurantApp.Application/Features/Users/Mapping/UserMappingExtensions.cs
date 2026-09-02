using System;
using MyRestaurantApp.Application.Features.Users.Dtos;
using MyRestaurantApp.Domain;


namespace MyRestaurantApp.Application.Features.Users.Mapping
{
    public static class UserMappingExtensions
    {
        // 1. Convert User Entity to UserResponse (Response Mapping)
        public static UserResponse ToResponse(this User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address ?? string.Empty,
                Role = user.Role.ToString()
            };
        }

        // 2. Convert RegisterRequest to User Entity (Request Mapping)
        public static User ToEntity(this RegisterRequest request, string passwordHash)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Password = passwordHash,
                Address = request.Address ?? string.Empty,
                Role = UserRole.Customer
            };
        }

        // 3. Convert CreateUserRequest to User Entity (Request Mapping for Admin)
        public static User ToEntity(this CreateUserRequest request, string passwordHash, Guid createdByUserId)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Password = passwordHash,
                Role = request.Role,
                Address = request.Address ?? string.Empty,
                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };
        }
     public static User ToEntity(this UpdateUserRequest request, User existingUser)
{
    if (request.FirstName != null)
        existingUser.FirstName = request.FirstName;

    if (request.LastName != null)
        existingUser.LastName = request.LastName;

    if (request.Email != null)
        existingUser.Email = request.Email;

    if (request.Phone != null)
        existingUser.Phone = request.Phone;
        
    if (request.Address != null)
     existingUser.Address = request.Address;

     return existingUser;

    
}
   
    }
}