using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Menus;

/// <summary>
/// Существует только в контексте Pizza (часть агрегата) - показывает сколько ингридиента в пицце
/// </summary>
public class PizzaIngredient : Entity<Guid>
{
    // Приватный конструктор без параметров для инфраструктуры
    private PizzaIngredient() : base() { }
    
    internal PizzaIngredient(
        Guid pizzaId,
        Guid ingredientId,
        string ingredientName,
        int quantity) : base(Guid.NewGuid())
    {
        PizzaId = pizzaId;
        IngredientId = ingredientId;
        IngredientName = ingredientName;
        Quantity = quantity;
    }

    public Guid PizzaId { get; private set; }
    public Guid IngredientId { get; private set; }
    public string IngredientName { get; private set; }
    public int Quantity { get; private set; }
    
    // Навигационные свойства
    public Pizza Pizza { get; private set; } = null!;
    public Ingredient Ingredient { get; private set; } = null!;

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));
        
        Quantity = newQuantity;
    }
}

