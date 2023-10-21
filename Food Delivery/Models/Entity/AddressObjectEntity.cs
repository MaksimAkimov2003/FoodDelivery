using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Entity;

public class AddressObjectEntity
{
    [Key] public Int64 id { get; set; }
    public Int64 objectid { get; set; }
    public Guid objectguid { get; set; }
    public Int64 changeid { get; set; }
    public string name { get; set; }
    public string typename { get; set; }
    public string level { get; set; }
    public int opertypeid { get; set; }
    public Int64 previd { get; set; }
    public Int64 nextid { get; set; }
    public DateTime updatedate { get; set; }
    public DateTime startdate { get; set; }
    public DateTime enddate { get; set; }
    public int isactual { get; set; }
    public int isactive { get; set; }
}