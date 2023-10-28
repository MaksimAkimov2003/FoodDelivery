using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Dto;

public class OrderInfoDto
{
    public Guid Id { get; set; }

    [Required] public DateTime DeliveryTime { get; set; }

    [Required] public DateTime OrderTime { get; set; }

    [Required] public string Status { get; set; }

    [Required] public double Price { get; set; }
}