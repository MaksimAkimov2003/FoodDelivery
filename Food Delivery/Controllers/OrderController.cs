using Food_Delivery.Common;
using Food_Delivery.Models.Dto;
using Food_Delivery.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Food_Delivery.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}")]
    [SwaggerOperation(Summary = "Get information about concrete order")]
    public async Task<IActionResult> GetOrderInfo(Guid id)
    {
        try
        {
            return Ok(await _orderService.GetOrderInfo(Guid.Parse(User.Identity.Name), id));
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

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [SwaggerOperation(Summary = "Get a list of orders")]
    public async Task<IActionResult> GetOrders()
    {
        try
        {
            return Ok(await _orderService.GetOrders(Guid.Parse(User.Identity.Name)));
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
    [SwaggerOperation(Summary = "Creating the order from dishes in basket")]
    public async Task CreateOrder([FromBody] OrderCreateDto orderCreateDto)
    {
        try
        {
            await _orderService.CreateOrder(Guid.Parse(User.Identity.Name), orderCreateDto);
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

    [HttpPost]
    [Authorize]
    [Authorize(Policy = "ValidateToken")]
    [Route("{id}/status")]
    [SwaggerOperation(Summary = "Confirm order delivery")]
    public async Task ConfirmOrderDelivery(Guid id)
    {
        try
        {
            await _orderService.ConfirmOrderDelivery(Guid.Parse(User.Identity.Name), id);
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