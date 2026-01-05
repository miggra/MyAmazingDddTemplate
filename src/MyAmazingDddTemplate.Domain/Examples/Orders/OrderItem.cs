using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Orders;

public class OrderItem : Entity<Guid>
{
    public string Name { get; private set; }
    public string Description {get; private set;}
    public int Quantity { get; private set; }
    public decimal OriginalPrice { get; private set; }
    public decimal ActualPrice { get; private set; }

    public string ImageUrl {get; private set;}
    public Order Order { get; private set; }
    public Guid OrderId { get; private set;}
    
}