using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Entity;

public class User
{
    public Guid Id { get; set; }

    [Required] [MinLength(1)] public string FullName { get; set; }

    public DateTime? BirthDate { get; set; }

    [Required] public string Gender { get; set; }

    [Phone] public string? PhoneNumber { get; set; }

    [Required]
    [MinLength(1)]
    [EmailAddress]
    public string Email { get; set; }

    public string? Address { get; set; }

    [Required] [MinLength(6)] public string Password { get; set; }

    public List<Rating> Ratings { get; set; }
    public List<Order> Orders { get; set; }
    public List<Cart> Carts { get; set; }
}