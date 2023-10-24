namespace Food_Delivery.Models.Entity;

public class AdministrativeHierarchyEntity
{
    public Int64 objectid { get; set; }
    public String path { get; set; }

    public Int64 parentobjid { get; set; }
}