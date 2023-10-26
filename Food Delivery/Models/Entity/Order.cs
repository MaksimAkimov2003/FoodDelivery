using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Food_Delivery.Models.Entity;

public class Order
{
    public Guid Id { get; set; }

    [Required] public DateTime DeliveryTime { get; set; }

    [Required] public DateTime OrderTime { get; set; }

    [Required] public string Status { get; set; }

    [Required] public double Price { get; set; }

    [Required] [MinLength(1)] public string Address { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required] [ForeignKey("UserId")] public User User { get; set; }

    public List<Cart> Carts { get; set; }
}