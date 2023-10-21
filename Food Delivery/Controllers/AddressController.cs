using Food_Delivery.Models.Dto;
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
    public string? GetAddressChain(Guid objectGuid)
    {
        return _service.GetAddressChain(objectGuid: objectGuid)?.name;
    }
}