using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Entity;

public class Dish
{
    public Guid Id { get; set; }

    [Required] [MinLength(1)] public string Name { get; set; }

    public string Description { get; set; }

    [Required] public double Price { get; set; }

    public double? Rating { get; set; }
    public string Image { get; set; }

    [Required] public bool Vegetarian { get; set; }

    [Required] public string Category { get; set; }

    public List<Cart> Carts { get; set; }
    public List<Rating> Ratings { get; set; }
}