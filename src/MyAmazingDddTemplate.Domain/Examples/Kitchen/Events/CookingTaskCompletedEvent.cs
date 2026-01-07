using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Kitchen.Events;

public record CookingTaskCompletedEvent(Guid CookingTaskId, Guid OrderId, Guid OrderItemId) : IDomainEvent;

