using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Orders;

public class Order : Entity<Guid>
{
    public OrderDeliveryAddress OrderDeliveryAddress {get; private set;}
}
