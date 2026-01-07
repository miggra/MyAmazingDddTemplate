using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Menus;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Orders;

/// <summary>
/// OrderItem - элемент заказа с immutable snapshot ингредиентов
/// После подтверждения заказа состав не меняется
/// </summary>
public class OrderItem : Entity<Guid>
{
    private readonly List<OrderItemIngredient> _ingredients = [];

    // Приватный конструктор без параметров для инфраструктуры
    private OrderItem() : base() { }

    private OrderItem(
        Guid menuItemId,
        Name name,
        string description,
        string imageUrl,
        int quantity) : base(Guid.NewGuid())
    {
        MenuItemId = menuItemId;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        Quantity = quantity;
    }

    // Ссылка на оригинальный элемент меню (для аналитики)
    public Guid MenuItemId { get; private set; }

    public Name Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;

    // Snapshot ингредиентов
    public IReadOnlyCollection<OrderItemIngredient> Ingredients
        => _ingredients.AsReadOnly();

    // Навигация
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    // Фабричный метод: создать из Pizza с стандартным рецептом
    public static OrderItem CreateFromPizza(
        Pizza pizza,
        int quantity,
        IEnumerable<Ingredient> allIngredients)
    {
        if (pizza == null)
            throw new ArgumentNullException(nameof(pizza));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var orderItem = new OrderItem(
            pizza.Id,
            pizza.Name,
            pizza.Description,
            pizza.ImageUrl,
            quantity
        );

        // Копируем стандартные ингредиенты из рецепта как snapshot
        foreach (var standardIngredient in pizza.StandardIngredients)
        {
            var ingredient = allIngredients.FirstOrDefault(i => i.Id == standardIngredient.IngredientId);
            if (ingredient == null)
                throw new InvalidOperationException($"Ingredient {standardIngredient.IngredientId} not found");

            orderItem._ingredients.Add(OrderItemIngredient.CreateSnapshot(
                ingredient.Id,
                ingredient.Name,
                ingredient.ImageUrl,
                standardIngredient.Quantity,
                ingredient.BasePrice
            ));
        }

        orderItem.RecalculatePrice();
        return orderItem;
    }

    // Кастомизация: добавить дополнительный ингредиент (ДО подтверждения заказа)
    public void AddExtraIngredient(Ingredient ingredient, int quantity = 1)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var existing = _ingredients.FirstOrDefault(i => i.IngredientId == ingredient.Id);

        if (existing != null)
        {
            // Увеличиваем количество (создаем новый immutable объект)
            _ingredients.Remove(existing);
            _ingredients.Add(OrderItemIngredient.CreateSnapshot(
                existing.IngredientId,
                existing.IngredientName,
                existing.IconUrl,
                existing.Quantity + quantity,
                existing.PriceSnapshot
            ));
        }
        else
        {
            // Добавляем новый ингредиент
            _ingredients.Add(OrderItemIngredient.CreateSnapshot(
                ingredient.Id,
                ingredient.Name, // Используем .Value из Name Value Object
                ingredient.ImageUrl,
                quantity,
                ingredient.BasePrice
            ));
        }

        RecalculatePrice();
    }

    // Кастомизация: удалить ингредиент (например, аллергия)
    public void RemoveIngredient(Guid ingredientId)
    {
        var ingredient = _ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
        if (ingredient != null)
        {
            _ingredients.Remove(ingredient);
            RecalculatePrice();
        }
    }

    // Обновление количества порций
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
        RecalculatePrice();
    }

    private void RecalculatePrice()
    {
        var ingredientsPrice = _ingredients.Sum(i => i.PriceSnapshot * i.Quantity);
        TotalPrice = ingredientsPrice * Quantity;
    }

    // Для отображения на кухне
    public string GetRecipeForKitchen()
    {
        return string.Join(", ",
            _ingredients.Select(i => $"{i.IngredientName} x{i.Quantity}"));
    }

    // Установить Order (используется при добавлении в заказ)
    internal void SetOrder(Order order)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        OrderId = order.Id;
    }
}