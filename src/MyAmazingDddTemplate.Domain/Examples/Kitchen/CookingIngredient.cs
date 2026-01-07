namespace MyAmazingDddTemplate.Domain.Examples.Kitchen;

/// <summary>
/// Value Object - ингредиент в задании для кухни
/// Immutable snapshot для отображения на кухонном экране
/// Record обеспечивает value-based equality и immutability
/// </summary>
public sealed record CookingIngredient
{
    // Приватный конструктор без параметров для инфраструктуры
    private CookingIngredient() { }

    private CookingIngredient(Guid ingredientId, string name, string iconUrl, int quantity)
    {
        IngredientId = ingredientId;
        Name = name;
        IconUrl = iconUrl;
        Quantity = quantity;
    }

    public Guid IngredientId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string IconUrl { get; init; } = string.Empty;
    public int Quantity { get; init; }

    // Фабричный метод с валидацией
    public static CookingIngredient Create(
        Guid ingredientId,
        string name,
        string iconUrl,
        int quantity)
    {
        if (ingredientId == Guid.Empty)
            throw new ArgumentException("Ingredient ID cannot be empty", nameof(ingredientId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        return new CookingIngredient(
            ingredientId,
            name,
            iconUrl ?? string.Empty,
            quantity
        );
    }
}

