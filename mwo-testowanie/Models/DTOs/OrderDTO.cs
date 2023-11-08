using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mwo_testowanie.Models.DTOs;

public class OrderDTO
{
    public Guid Id { get; set; }
    public OrderState State { get; set; }
    public Guid ClientId { get; set; }
    public virtual List<ProductDTO> Products { get; set; }
}