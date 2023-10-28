using Food_Delivery.Models.Dto;

namespace Food_Delivery.Services.Basket;

public interface IBasketService
{
    Task<List<DishBasketDto>> GetUserCart(Guid userId);
    Task AddDishToCart(Guid dishId, Guid userId);
    Task RemoveDishFromCart(Guid dishId, Guid userId, bool increase);
}