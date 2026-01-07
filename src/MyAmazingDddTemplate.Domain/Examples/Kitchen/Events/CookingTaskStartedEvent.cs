using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Kitchen.Events;

public record CookingTaskStartedEvent(Guid CookingTaskId, Guid OrderItemId) : IDomainEvent;

