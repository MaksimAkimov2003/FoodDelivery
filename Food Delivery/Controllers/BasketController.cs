using Food_Delivery.Common;
using Food_Delivery.Services.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Food_Delivery.Controllers;

[ApiController]
[Route("api/basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [SwaggerOperation(Summary = "Get user cart")]
    public async Task<IActionResult> GetUserCart()
    {
        try
        {
            return Ok(await _basketService.GetUserCart(Guid.Parse(User.Identity.Name)));
        }

        catch (AuthException e)
        {
            return Unauthorized(new StatusResponse { Message = e.Message });
        }

        catch (Exception e)
        {
            return BadRequest(new StatusResponse { Message = e.Message });
        }
    }

    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("dish/{dishId}")]
    [SwaggerOperation(Summary = "Add dish to cart")]
    public async Task AddDishToCart(Guid dishId)
    {
        try
        {
            await _basketService.AddDishToCart(dishId, Guid.Parse(User.Identity.Name));
        }

        catch (AuthException e)
        {
            Unauthorized(new StatusResponse { Message = e.Message });
        }

        catch (Exception e)
        {
            BadRequest(new StatusResponse { Message = e.Message });
        }
    }

    [HttpDelete]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("dish/{dishId}")]
    [SwaggerOperation(Summary =
        "Decrease the number of dishes in the cart(if increase = true), or remove the dish completely(increase = false)")]
    public async Task DecreaseDishQuantityInCart(Guid dishId, bool increase = false)
    {
        try
        {
            await _basketService.RemoveDishFromCart(dishId, Guid.Parse(User.Identity.Name), increase);
        }

        catch (AuthException e)
        {
            Unauthorized(new StatusResponse { Message = e.Message });
        }

        catch (Exception e)
        {
            BadRequest(new StatusResponse { Message = e.Message });
        }
    }
}