using Food_Delivery.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Food_Delivery;

public class AddressDbContext : DbContext
{
    public AddressDbContext(DbContextOptions<AddressDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddressObjectEntity>().HasNoKey();
    }
}