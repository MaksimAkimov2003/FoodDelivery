using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Dto;

public class LoginCredentials
{
    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }

    [Required] [MinLength(1)] public string Password { get; set; }
}