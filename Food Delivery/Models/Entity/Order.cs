using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Food_Delivery.Models.Entity;

public class Order
{
    public Guid Id { get; set; }

    public DateTime DeliveryTime { get; set; }

    public DateTime OrderTime { get; set; }

    public string Status { get; set; }

    public double Price { get; set; }

    public Guid Address { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required] [ForeignKey("UserId")] public User User { get; set; }

    public List<Cart> Carts { get; set; }
}