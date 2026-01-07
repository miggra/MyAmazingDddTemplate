using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Orders;

public enum OrderStatus
{
    Draft = 0,          // В корзине
    Confirmed = 1,      // Подтвержден
    InProgress = 2,     // В процессе приготовления
    Ready = 3,          // Готов
    Delivered = 4,      // Доставлен
    Cancelled = 5       // Отменен
}

public class Order : Entity<Guid>, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = [];
    
    // Приватный конструктор без параметров для инфраструктуры
    private Order() : base() { }

    private Order(DeliveryAddress deliveryAddress) : base(Guid.NewGuid())
    {
        OrderDeliveryAddress = deliveryAddress;
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public OrderStatus Status { get; private set; }
    public DeliveryAddress OrderDeliveryAddress { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public decimal TotalAmount { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Order Create(DeliveryAddress deliveryAddress)
    {
        if (deliveryAddress == null)
            throw new ArgumentNullException(nameof(deliveryAddress));

        return new Order(deliveryAddress);
    }

    public void AddOrderItem(OrderItem orderItem)
    {
        if (orderItem == null)
            throw new ArgumentNullException(nameof(orderItem));

        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify confirmed order");

        orderItem.SetOrder(this);
        _orderItems.Add(orderItem);
        RecalculateTotalAmount();
    }

    public void RemoveOrderItem(OrderItem orderItem)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify confirmed order");

        if (_orderItems.Remove(orderItem))
        {
            RecalculateTotalAmount();
        }
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order already confirmed");

        if (!_orderItems.Any())
            throw new InvalidOperationException("Cannot confirm empty order");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        RaiseDomainEvent(new Events.OrderConfirmedEvent(Id, _orderItems.ToList()));
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Order must be confirmed first");

        Status = OrderStatus.InProgress;
    }

    public void MarkAsReady()
    {
        if (Status != OrderStatus.InProgress)
            throw new InvalidOperationException("Order must be in progress");

        Status = OrderStatus.Ready;
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Ready)
            throw new InvalidOperationException("Order must be ready");

        Status = OrderStatus.Delivered;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel delivered order");

        Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotalAmount()
    {
        TotalAmount = _orderItems.Sum(oi => oi.TotalPrice);
    }
}
