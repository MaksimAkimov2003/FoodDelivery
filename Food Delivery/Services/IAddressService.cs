using Food_Delivery.Models.Dto;
using Food_Delivery.Models.Entity;

namespace Food_Delivery.Services;

public interface IAddressService
{
    public AddressObjectEntity? GetAddressChain(Guid objectGuid);
}