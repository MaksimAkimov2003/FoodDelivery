using Food_Delivery.Common;
using Food_Delivery.Models.Dto;
using Food_Delivery.Services.Dish;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Food_Delivery.Controllers;

[ApiController]
[Route("api/dish")]
public class DishController : ControllerBase
{
    private readonly IDishService _dishService;

    public DishController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get a list of dishes (menu)")]
    public async Task<IActionResult> GetDishList([FromQuery] GetDishListQuery dishListQuery)
    {
        try
        {
            return Ok(await _dishService.GetDishList(dishListQuery));
        }
        catch (Exception e)
        {
            return BadRequest(new StatusResponse { Message = e.Message });
        }
    }

    [HttpGet]
    [Route("{id}")]
    [SwaggerOperation(Summary = "Get information about concrete dish")]
    public async Task<IActionResult> GetDish(Guid id)
    {
        try
        {
            return Ok(await _dishService.GetDish(id));
        }
        catch (Exception e)
        {
            return BadRequest(new StatusResponse { Message = e.Message });
        }
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}/rating/check")]
    [SwaggerOperation(Summary = "Checks if user is able to set rating of the dish")]
    public async Task<IActionResult> CheckDishRating(Guid id)
    {
        try
        {
            return Ok(await _dishService.CheckDishRating(id, Guid.Parse(User.Identity.Name)));
        }

        catch (Exception e)
        {
            return BadRequest(new StatusResponse { Message = e.Message });
        }
    }

    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}/rating")]
    [SwaggerOperation(Summary = "Set a rating for a dish")]
    public async Task SetDishRating(Guid id, [FromQuery] int rating)
    {
        try
        {
            await _dishService.SetDishRating(id, rating, Guid.Parse(User.Identity.Name));
        }

        catch (Exception e)
        {
            BadRequest(new StatusResponse { Message = e.Message });
        }
    }
}