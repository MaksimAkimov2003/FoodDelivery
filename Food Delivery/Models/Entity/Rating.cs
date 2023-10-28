using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Food_Delivery.Models.Entity;

public class Rating
{
    public Guid Id { get; set; }

    [Required] public Guid DishId { get; set; }

    [Required] [ForeignKey("DishId")] public Dish Dish { get; set; }

    [Required] public Guid UserId { get; set; }

    [Required] [ForeignKey("UserId")] public User User { get; set; }

    [Required] public int RatingScore { get; set; }
}