using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Dto;

public class DishPagedListDto
{
    [Required] public List<DishDto> Dishes { get; set; }

    [Required] public PageInfoModel Pagination { get; set; }
}