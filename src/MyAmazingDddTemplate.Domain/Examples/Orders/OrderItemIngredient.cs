using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Orders;

/// <summary>
/// Value Object - ингредиент в заказе (immutable snapshot)
/// Record обеспечивает value-based equality и immutability
/// </summary>
public sealed record OrderItemIngredient
{
    // Приватный конструктор без параметров для инфраструктуры
    private OrderItemIngredient() { }

    private OrderItemIngredient(
        Guid ingredientId,
        Name ingredientName,
        string iconUrl,
        int quantity,
        decimal priceSnapshot)
    {
        IngredientId = ingredientId;
        IngredientName = ingredientName;
        IconUrl = iconUrl;
        Quantity = quantity;
        PriceSnapshot = priceSnapshot;
    }

    public Guid IngredientId { get; init; }
    public Name IngredientName { get; init; }
    public string IconUrl { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal PriceSnapshot { get; init; }

    // Фабричный метод с валидацией - единственный способ создания
    public static OrderItemIngredient CreateSnapshot(
        Guid ingredientId,
        Name ingredientName,
        string iconUrl,
        int quantity,
        decimal priceSnapshot)
    {
        if (ingredientId == Guid.Empty)
            throw new ArgumentException("Ingredient ID cannot be empty", nameof(ingredientId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (priceSnapshot < 0)
            throw new ArgumentException("Price cannot be negative", nameof(priceSnapshot));


        return new OrderItemIngredient(
            ingredientId,
            ingredientName,
            iconUrl ?? string.Empty,
            quantity,
            priceSnapshot
        );
    }
}

