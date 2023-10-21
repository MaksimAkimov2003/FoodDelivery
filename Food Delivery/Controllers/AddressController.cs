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