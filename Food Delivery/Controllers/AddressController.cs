using Food_Delivery.Common;
using Food_Delivery.Services;
using Microsoft.AspNetCore.Mvc;

namespace Food_Delivery.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressController : ControllerBase
{
    private readonly IAddressService _service;

    public AddressController(IAddressService service)
    {
        _service = service;
    }

    [HttpGet]
    [Route("api/address/search")]
    public IActionResult SearchAddress
    (
        long? parentObjectId,
        string? query
    )
    {
        try
        {
            return Ok(_service.SearchAddress(parentObjectId: parentObjectId ?? 0, query: query ?? ""));
        }
        catch (Exception e)
        {
            return StatusCode(500, new StatusResponse { Message = "object wasn't found" });
        }
    }

    [HttpGet]
    [Route("/api/address/getaddresschain")]
    public IActionResult GetAddressChain(Guid objectGuid)
    {
        try
        {
            return Ok(_service.GetAddressChain(objectGuid));
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
}