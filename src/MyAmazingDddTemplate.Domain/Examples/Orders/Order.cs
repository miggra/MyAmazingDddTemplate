using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Orders;

public class Order : Entity<Guid>
{
    private readonly List<OrderItem> _orderItems = [];
    public DeliveryAddress OrderDeliveryAddress {get; private set;}

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public void AddOrderItem(OrderItem orderItem)
    {
        _orderItems.Add(orderItem);
    }

    public void RemoveOrderItem(OrderItem orderItem)
    {
        _orderItems.Remove(orderItem);
    }
}
