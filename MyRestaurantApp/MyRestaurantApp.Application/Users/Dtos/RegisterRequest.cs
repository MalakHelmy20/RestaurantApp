using System.ComponentModel.DataAnnotations;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Users.Dtos
{
    public class RegisterRequest
    {
     

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string confirmPassword { get; set; } = null!;
    }
}