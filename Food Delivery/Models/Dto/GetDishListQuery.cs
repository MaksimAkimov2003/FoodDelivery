namespace Food_Delivery.Models.Dto;

public class GetDishListQuery
{
    public List<string> Categories { get; set; } = new();
    public bool? Vegetarian { get; set; } = null;
    public string? Sorting { get; set; } = null;
    public int Page { get; set; } = 1;
}