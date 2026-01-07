using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Orders;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Kitchen;

/// <summary>
/// CookingTask - Aggregate Root для задания на кухне
/// </summary>
public class CookingTask : Entity<Guid>, IAggregateRoot
{
    private readonly List<CookingIngredient> _requiredIngredients = [];

    // Приватный конструктор без параметров для инфраструктуры
    private CookingTask() : base() { }

    private CookingTask(
        Guid orderId,
        Guid orderItemId,
        Name dishName,
        int quantity,
        IEnumerable<CookingIngredient> ingredients) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        OrderItemId = orderItemId;
        DishName = dishName;
        Quantity = quantity;
        Status = CookingTaskStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        foreach (var ingredient in ingredients)
        {
            _requiredIngredients.Add(ingredient);
        }
    }

    public Guid OrderId { get; private set; }
    public Guid OrderItemId { get; private set; }
    public Name DishName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public CookingTaskStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Snapshot ингредиентов для кухни
    public IReadOnlyCollection<CookingIngredient> RequiredIngredients
        => _requiredIngredients.AsReadOnly();

    // Фабричный метод: создать из OrderItem
    public static CookingTask CreateFromOrderItem(OrderItem orderItem)
    {
        if (orderItem == null)
            throw new ArgumentNullException(nameof(orderItem));

        // Копируем ингредиенты из заказа с умножением на количество порций
        var ingredients = orderItem.Ingredients
            .Select(oi => CookingIngredient.Create(
                oi.IngredientId,
                oi.IngredientName,
                oi.IconUrl,
                oi.Quantity * orderItem.Quantity // Умножаем на количество порций
            ))
            .ToList();

        return new CookingTask(
            orderItem.OrderId,
            orderItem.Id,
            orderItem.Name,
            orderItem.Quantity,
            ingredients
        );
    }

    // Управление статусом
    public void StartCooking()
    {
        if (Status != CookingTaskStatus.Pending)
            throw new InvalidOperationException("Task already started or completed");

        Status = CookingTaskStatus.InProgress;
        StartedAt = DateTime.UtcNow;

        RaiseDomainEvent(new Events.CookingTaskStartedEvent(Id, OrderItemId));
    }

    public void Complete()
    {
        if (Status != CookingTaskStatus.InProgress)
            throw new InvalidOperationException("Task must be in progress to complete");

        Status = CookingTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        RaiseDomainEvent(new Events.CookingTaskCompletedEvent(Id, OrderId, OrderItemId));
    }

    public void Cancel(string reason)
    {
        if (Status == CookingTaskStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed task");

        Status = CookingTaskStatus.Cancelled;
    }

    // Бизнес-логика для кухни
    public string GetIngredientsDisplay()
    {
        return string.Join(", ",
            _requiredIngredients.Select(i => $"{i.Name} x{i.Quantity}"));
    }

    public TimeSpan? GetCookingDuration()
    {
        if (StartedAt.HasValue && CompletedAt.HasValue)
        {
            return CompletedAt.Value - StartedAt.Value;
        }
        return null;
    }

    public bool IsOverdue(int maxMinutes)
    {
        if (Status != CookingTaskStatus.InProgress || !StartedAt.HasValue)
            return false;

        var elapsed = DateTime.UtcNow - StartedAt.Value;
        return elapsed.TotalMinutes > maxMinutes;
    }
}

