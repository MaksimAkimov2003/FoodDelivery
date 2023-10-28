using Food_Delivery.Models.Dto;

namespace Food_Delivery.Services.Dish;

public interface IDishService
{
    Task<DishPagedListDto> GetDishList(GetDishListQuery dishListQuery);
    Task<DishDto> GetDish(Guid dishId);
    Task<bool> CheckDishRating(Guid id, Guid userId);
    Task SetDishRating(Guid id, int rating, Guid userId);
}