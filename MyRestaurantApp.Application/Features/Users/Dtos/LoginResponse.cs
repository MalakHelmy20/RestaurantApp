using MyRestaurantApp.Domain;

namespace MyRestaurantApp.Application.Features.Users.Dtos
{
public class LoginResponse
{
    public UserResponse User { get; set; } = null!;
    public string Token { get; set; } = null!;
}
}