using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mwo_testowanie.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
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
    
    public virtual List<ProductQuantity> Orders { get; set; }
}