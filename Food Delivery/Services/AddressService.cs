using Food_Delivery.Models.Dto;
using Food_Delivery.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Food_Delivery.Services;

public class AddressService : IAddressService
{
    private readonly AddressDbContext _context;

    public AddressService(AddressDbContext context)
    {
        _context = context;
    }

    public AddressObjectEntity? GetAddressChain(Guid objectGuid)
    {
        var objectGuidParam = new NpgsqlParameter("objectGuid", objectGuid);
    
        return _context.Set<AddressObjectEntity>()
            .FromSqlRaw("SELECT * FROM fias.as_addr_obj WHERE objectguid=@objectGuid", objectGuidParam)
            .FirstOrDefault();
    }

    private Int64 GetObjectId(Guid objectGuid)
    {
        return 5;
    }
}