using Food_Delivery.Common.db;
using Food_Delivery.Models.Dto;
using Food_Delivery.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Food_Delivery.Services.Basket;

public class BasketService : IBasketService
{
    private readonly ApplicationDbContext _context;

    public BasketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DishBasketDto>> GetUserCart(Guid userId)
    {
        var dishList = await _context.Carts.Where(x => x.UserId == userId && x.OrderId == null).Join(
                _context.Dishes,
                c => c.DishId,
                d => d.Id,
                (c, d) => new DishBasketDto
                {
                    Id = c.Id,
                    Name = d.Name,
                    Price = d.Price,
                    TotalPrice = d.Price * c.Amount,
                    Amount = c.Amount,
                    Image = d.Image
                }
            )
            .ToListAsync();

        return dishList;
    }

    public async Task AddDishToCart(Guid dishId, Guid userId)
    {
        if (await _context.Dishes.FirstOrDefaultAsync(x => x.Id == dishId) == null)
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Dish not exists"
            );
            throw ex;
        }

        var dishCartEntity =
            await _context.Carts.Where(x => x.UserId == userId && x.DishId == dishId && x.OrderId == null)
                .FirstOrDefaultAsync();

        if (dishCartEntity == null)
        {
            await _context.Carts.AddAsync(new Cart
            {
                Id = Guid.NewGuid(),
                DishId = dishId,
                Amount = 1,
                UserId = userId,
                OrderId = null
            });
            await _context.SaveChangesAsync();
        }
        else
        {
            dishCartEntity.Amount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveDishFromCart(Guid dishId, Guid userId, bool increase)
    {
        if (await _context.Dishes.FirstOrDefaultAsync(x => x.Id == dishId) == null)
        {
            var ex = new Exception("Dish not exists");
            throw ex;
        }

        var dishCartEntity =
            await _context.Carts.Where(x => x.UserId == userId && x.DishId == dishId && x.OrderId == null)
                .FirstOrDefaultAsync();

        if (dishCartEntity == null)
        {
            var ex = new Exception("Dish not found in cart");
            throw ex;
        }

        if (!increase)
        {
            dishCartEntity.Amount--;
            if (dishCartEntity.Amount == 0)
                _context.Carts.Remove(dishCartEntity);
        }
        else
        {
            _context.Carts.Remove(dishCartEntity);
        }

        await _context.SaveChangesAsync();
    }
}