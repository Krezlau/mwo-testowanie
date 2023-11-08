using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mwo_testowanie.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    public OrderState State { get; set; }
    
    public Guid UserId { get; set; }
    
    [Required]
    [ForeignKey(nameof(UserId))]
    public virtual Client Client { get; set; }
    
    public virtual List<ProductQuantity> Products { get; set; }
}
