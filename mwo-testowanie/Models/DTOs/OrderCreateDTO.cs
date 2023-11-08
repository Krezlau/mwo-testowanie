using System.ComponentModel.DataAnnotations;

namespace mwo_testowanie.Models.DTOs;

public class OrderCreateDTO
{
    [Required]
    public Guid ClientId { get; set; }
    [Required]
    public List<Guid> ProductIds { get; set; }
}
