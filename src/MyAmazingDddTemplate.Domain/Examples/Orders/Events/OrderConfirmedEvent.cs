using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Orders.Events;

public record OrderConfirmedEvent(Guid OrderId, IReadOnlyList<OrderItem> OrderItems) : IDomainEvent;

