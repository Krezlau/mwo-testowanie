using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mwo_testowanie.Models.DTOs;

public class OrderDTO
{
    public Guid Id { get; set; }
    public OrderState State { get; set; }
    public Guid ClientId { get; set; }
    public virtual List<(ProductDTO product, int quantity)> Products { get; set; }

    protected bool Equals(OrderDTO other)
    {
        return Id.Equals(other.Id) && State == other.State && ClientId.Equals(other.ClientId);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((OrderDTO)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int)State, ClientId, Products);
    }
}