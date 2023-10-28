using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Dto;

public class UserRegisterModel
{
    [Required] [MinLength(1)] public string FullName { get; set; }

    [Required] [MinLength(6)] public string Password { get; set; }

    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }

    public Guid Address { get; set; }
    public DateTime? BirthDate { get; set; }

    [Required] public string Gender { get; set; }

    [Phone] public string? PhoneNumber { get; set; }
}