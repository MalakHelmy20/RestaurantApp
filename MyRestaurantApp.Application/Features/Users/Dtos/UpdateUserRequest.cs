using System;
using System.ComponentModel.DataAnnotations;

namespace MyRestaurantApp.Application.Features.Users.Dtos
{
    public class UpdateUserRequest
    {

 [StringLength(100, MinimumLength = 2, ErrorMessage = "FirstName must be between 2 and 100 characters.")]
    public string? FirstName { get; set; }


  [StringLength(100, MinimumLength = 2, ErrorMessage = "LastName must be between 2 and 100 characters.")]
    public string? LastName { get; set; }



  [EmailAddress(ErrorMessage = "Invalid email format.")] // .Net uses EmailAddressAttribute to validate email format
    public string? Email { get; set; }

[Phone(ErrorMessage = "Invalid phone format.")]
    public string? Phone { get; set; } // .Net uses phone attribute to validate phone number format


[RegularExpression(
    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
    ErrorMessage = "Password must be 8+ chars with upper, lower, number & symbol.")]
    public string? Password { get; set; }
}
}