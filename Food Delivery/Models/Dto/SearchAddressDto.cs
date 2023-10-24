namespace Food_Delivery.Models.Dto;

public class SearchAddressDto
{
    public Int64 ObjectId { get; set; }
    public Guid ObjectGuid { get; set; }
    public string? Text { get; set; }
    public GarAddressLevel ObjectLevel { get; set; }

    public string ObjectLevelText { get; set; }
}