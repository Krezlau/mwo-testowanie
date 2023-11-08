using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mwo_testowanie.Models;
public class ProductQuantity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [ForeignKey(nameof(ProductId))]
    public Guid ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    [ForeignKey(nameof(OrderId))]
    public Guid OrderId { get; set; }
    
    public virtual Product Product { get; set; }
    public virtual Order Order { get; set; }
}