using Food_Delivery.Models.Dto;

namespace Food_Delivery.Services.Order;

public interface IOrderService
{
    public Task<OrderDto> GetOrderInfo(Guid userId, Guid orderId);
    public Task<List<OrderInfoDto>> GetOrders(Guid userId);
    public Task CreateOrder(Guid userId, OrderCreateDto orderCreateDto);
    public Task ConfirmOrderDelivery(Guid userId, Guid orderId);
}