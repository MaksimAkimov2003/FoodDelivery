using Food_Delivery.Common.db;
using Food_Delivery.Models.Dto;
using Food_Delivery.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace Food_Delivery.Services.Order;

public class OrderService : IOrderService
{
    private readonly AddressDbContext _addressContext;
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context, AddressDbContext addressContext)
    {
        _context = context;
        _addressContext = addressContext;
    }

    public async Task<OrderDto> GetOrderInfo(Guid userId, Guid orderId)
    {
        var orderInfo = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (orderInfo == null)
        {
            var ex = new Exception("Order Info not found");
            throw ex;
        }

        if (orderInfo.UserId != userId)
        {
            var ex = new Exception("Invalid order owner");
            throw ex;
        }

        var orderCarts = await _context.Carts.Where(x => x.OrderId == orderId).ToListAsync();
        if (orderCarts.IsNullOrEmpty())
        {
            var ex = new Exception("Dishes in Order not found");
            throw ex;
        }

        var dishes = new List<Models.Entity.Dish>();
        foreach (var orderCart in orderCarts)
        {
            var dish = await _context.Dishes.FirstOrDefaultAsync(x => x.Id == orderCart.DishId);

            if (dish != null)
                dishes.Add(dish);
            else
            {
                var ex = new Exception("Dish in Order not found");
                throw ex;
            }
        }

        var convertedDishes = (from orderCart in orderCarts
            let dish = dishes.FirstOrDefault(x => x.Id == orderCart.DishId)
            where dish != null
            select new DishBasketDto
            {
                Id = dish.Id,
                Name = dish.Name,
                Price = dish.Price,
                TotalPrice = orderCart.Amount * dish.Price,
                Amount = orderCart.Amount,
                Image = dish.Image
            }).ToList();

        if (!convertedDishes.IsNullOrEmpty())
            return new OrderDto
            {
                Id = orderInfo.Id,
                DeliveryTime = orderInfo.DeliveryTime,
                OrderTime = orderInfo.OrderTime,
                Status = orderInfo.Status,
                Price = orderInfo.Price,
                Dishes = convertedDishes,
                Address = orderInfo.Address
            };
        {
            var ex = new Exception("Empty order list returned");
            throw ex;
        }
    }

    public async Task<List<OrderInfoDto>> GetOrders(Guid userId)
    {
        var orders = await _context.Orders.Where(x => x.UserId == userId).ToListAsync();

        return orders.Select(order =>
            new OrderInfoDto
            {
                DeliveryTime = order.DeliveryTime,
                Id = order.Id,
                OrderTime = order.OrderTime,
                Price = order.Price,
                Status = order.Status
            }
        ).ToList();
    }

    public async Task CreateOrder(Guid userId, OrderCreateDto orderCreateDto)
    {
        if (orderCreateDto.DeliveryTime - DateTime.Now < TimeSpan.FromMinutes(5) ||
            orderCreateDto.DeliveryTime - DateTime.Now > TimeSpan.FromHours(24))
        {
            var ex = new Exception("Bad request, Delivery time range is 5m - 24h");
            throw ex;
        }

        CheckAddress(houseGuid: orderCreateDto.Address);

        var cartDishes = await _context.Carts
            .Where(x => x.UserId == userId && x.OrderId == null)
            .ToListAsync();
        if (cartDishes.IsNullOrEmpty())
        {
            var ex = new Exception("Dishes in cart Not Found");
            throw ex;
        }

        // В бд всем товарам в корзине проставить, что теперь они в заказе,
        // заодно посчитать стоимость
        var orderId = Guid.NewGuid();
        var newOrder = new Models.Entity.Order
        {
            Id = orderId,
            DeliveryTime = orderCreateDto.DeliveryTime,
            OrderTime = DateTime.UtcNow,
            Status = OrderStatus.InProcess.ToString(),
            Price = 0,
            Address = orderCreateDto.Address,
            UserId = userId
        };
        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
        newOrder.Price = await CreateOrderOperations(orderId, cartDishes);
        await _context.SaveChangesAsync();
    }

    public async Task ConfirmOrderDelivery(Guid userId, Guid orderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);

        if (order == null)
        {
            var ex = new Exception("Order Info not found");
            throw ex;
        }

        if (order.UserId != userId)
        {
            var ex = new Exception("Invalid order owner");
            throw ex;
        }

        order.Status = OrderStatus.Delivered.ToString();
        await _context.SaveChangesAsync();
    }

    private async Task<double> CreateOrderOperations(Guid orderId, IReadOnlyList<Cart> cartDishes)
    {
        double res = 0;

        for (var i = 0; i < cartDishes.Count; i++)
        {
            cartDishes[i].OrderId = orderId;
            var dish = await _context.Dishes.FirstOrDefaultAsync(x => x.Id == cartDishes[i].DishId);
            if (dish == null)
            {
                var ex = new Exception("Dish in Order not found");
                throw ex;
            }

            res += cartDishes[i].Amount * dish.Price;
        }

        await _context.SaveChangesAsync();

        return res;
    }

    private void CheckAddress(Guid houseGuid)
    {
        var objectGuidParam = new NpgsqlParameter("objectGuid", houseGuid);

        var houseEntity = _addressContext.Set<HouseEntity>()
            .FromSqlRaw("SELECT * FROM fias.as_houses WHERE objectguid=@objectGuid", objectGuidParam)
            .FirstOrDefault();

        if (houseEntity == null)
        {
            throw new Exception("House guid expected in address field");
        }
    }
}