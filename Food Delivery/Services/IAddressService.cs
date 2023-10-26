using Food_Delivery.Models.Dto;

namespace Food_Delivery.Services;

public interface IAddressService
{
    public List<SearchAddressDto> GetAddressChain(Guid objectGuid);

    public List<SearchAddressDto> SearchAddress(long parentObjectId, string query);
}