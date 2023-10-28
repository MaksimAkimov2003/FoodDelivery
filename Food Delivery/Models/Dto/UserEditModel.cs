using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Dto;

public class UserEditModel
{
    [Required] [MinLength(1)] public string FullName { get; set; }

    public DateTime? BirthDate { get; set; }

    [Required] public string Gender { get; set; }

    public Guid Address { get; set; }

    [Phone] public string? PhoneNumber { get; set; }
}