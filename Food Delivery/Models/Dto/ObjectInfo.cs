namespace Food_Delivery.Models.Dto;

public abstract class ObjectInfo
{
    public Int64 Id { get; set; }
    public Guid Guid { get; set; }

    public class AddressObject : ObjectInfo
    {
        public string Level { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
    }

    public class House : ObjectInfo
    {
        public string HouseNum { get; set; }
        public string? AddNum1 { get; set; }
        public string? AddNum2 { get; set; }
    }
}