using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Users.Dtos
{
    public class CreateUserRequest
    {
        public  required string FirstName { get; set; } = string.Empty;

        public  required string LastName { get; set; } = string.Empty;

        public  required string Email { get; set; } = string.Empty;

        public  required string Phone { get; set; } = string.Empty;

        public  required string Password { get; set; } = string.Empty;

        public  required UserRole Role { get; set; }

     
    }
}