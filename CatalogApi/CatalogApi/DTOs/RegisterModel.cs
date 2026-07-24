using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CatalogApi.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "Username is required")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    public string? UserName { get; set; }
    
    [EmailAddress(ErrorMessage = "Email is invalid")]
    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string? Password { get; set; }
}