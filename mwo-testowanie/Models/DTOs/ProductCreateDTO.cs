using System.ComponentModel.DataAnnotations;

namespace mwo_testowanie.Models.DTOs;

public class ProductCreateDTO
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Required]
    [Range(0.00, double.MaxValue)]
    public double Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityLeft { get; set; }
}