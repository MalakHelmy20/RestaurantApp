
using System.ComponentModel.DataAnnotations;
namespace MyRestaurantApp.Application.Features.Users.Dtos
{
    public class LoginRequest
    {

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = null!; 
    }
}