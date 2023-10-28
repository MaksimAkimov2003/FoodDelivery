using System.ComponentModel.DataAnnotations;

namespace Food_Delivery.Models.Entity;

public class Token
{
    [Required] public string InvalidToken { get; set; }

    [Required] public DateTime ExpiredDate { get; set; }
}