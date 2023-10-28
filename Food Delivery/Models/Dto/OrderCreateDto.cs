using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Dto;

public class OrderCreateDto
{
    [Required] public DateTime DeliveryTime { get; set; }

    [Required] public Guid Address { get; set; }
}