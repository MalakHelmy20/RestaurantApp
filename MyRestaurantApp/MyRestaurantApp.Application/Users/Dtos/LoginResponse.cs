using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Users.Dtos
{
public class LoginResponse
{
    public Guid Id { get; set; }
    //public Guid CreatedByUserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;


    public string Phone { get; set; } = null!;
    public string Role { get; set; } = null!; // or enum UserRole
    public string Token { get; set; } = null!;
}
}