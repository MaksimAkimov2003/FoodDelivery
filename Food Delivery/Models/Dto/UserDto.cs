using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Dto;

public class UserDto
{
    public Guid Id { get; set; }

    [Required] [MinLength(1)] public string FullName { get; set; }

    public DateTime? BirthDate { get; set; }

    [Required] public string Gender { get; set; }

    public string? Address { get; set; }

    [MinLength(1)] [EmailAddress] public string? Email { get; set; }

    [Phone] public string? PhoneNumber { get; set; }
}