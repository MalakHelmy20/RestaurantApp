using System.ComponentModel.DataAnnotations;
using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Users.Dtos
{
    public class RegisterRequest
    {
     
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = null!;


        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is not valid.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; } = null!;

      [Required(ErrorMessage = "Password is required.")]
      [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
      [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]).*$", 
        ErrorMessage = "Password must contain at least one uppercase letter and one special character (!@#$%^&*).")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string confirmPassword { get; set; } = null!;
    }
}