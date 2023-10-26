using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Entity;

public class AddressObjectEntity
{
    [Key] public Int64 id { get; set; }
    public Int64 objectid { get; set; }
    public Guid objectguid { get; set; }
    public string name { get; set; }
    public string typename { get; set; }
    public string level { get; set; }
}