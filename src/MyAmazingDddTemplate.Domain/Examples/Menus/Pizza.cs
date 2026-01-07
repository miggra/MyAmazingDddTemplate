using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Menus;

/// <summary>
/// Пицца в меню - Aggregate Root с изменяемым рецептом
/// Управляет коллекцией стандартных ингредиентов
/// </summary>
public class Pizza : MenuItem
{
    private readonly List<PizzaIngredient> _standardIngredients = [];

    // Приватный конструктор без параметров для инфраструктуры
    private Pizza() : base() { }

    private Pizza(
        Name name,
        string description,
        decimal defaultPrice,
        string imageUrl,
        PizzaSize size,
        CrustType crust) : base()
    {
        Name = name;
        Description = description;
        DefaultPrice = defaultPrice;
        ImageUrl = imageUrl;
        Size = size;
        Crust = crust;
    }

    public override MenuCategory Category => MenuCategory.Pizza;
    public PizzaSize Size { get; private set; } = null!;
    public CrustType Crust { get; private set; } = null!;

    // Стандартный рецепт (может изменяться)
    public IReadOnlyCollection<PizzaIngredient> StandardIngredients
        => _standardIngredients.AsReadOnly();

    // Фабричный метод
    public static Pizza Create(
        Name name,
        string description,
        decimal defaultPrice,
        string imageUrl,
        PizzaSize size,
        CrustType crust)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (defaultPrice <= 0)
            throw new ArgumentException("Default price must be positive", nameof(defaultPrice));

        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty", nameof(imageUrl));

        return new Pizza(name, description ?? string.Empty, defaultPrice, imageUrl, size, crust);
    }

    // Управление ингредиентами в рецепте
    public void AddIngredient(Ingredient ingredient, int quantity)
    {
        if (ingredient == null)
            throw new ArgumentNullException(nameof(ingredient));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        // Проверка на дубликат
        if (_standardIngredients.Any(pi => pi.IngredientId == ingredient.Id))
            throw new InvalidOperationException($"Ingredient '{ingredient.Name.Value}' already exists in recipe");

        var pizzaIngredient = new PizzaIngredient(
            this.Id,
            ingredient.Id,
            ingredient.Name.Value, // Используем .Value из Name Value Object
            quantity
        );

        _standardIngredients.Add(pizzaIngredient);
    }

    public void RemoveIngredient(Guid ingredientId)
    {
        var ingredient = _standardIngredients.FirstOrDefault(pi => pi.IngredientId == ingredientId);
        if (ingredient != null)
        {
            _standardIngredients.Remove(ingredient);
        }
    }

    public void UpdateIngredientQuantity(Guid ingredientId, int newQuantity)
    {
        var ingredient = _standardIngredients.FirstOrDefault(pi => pi.IngredientId == ingredientId);
        if (ingredient == null)
            throw new InvalidOperationException("Ingredient not found in recipe");

        ingredient.UpdateQuantity(newQuantity);
    }

    // Обновление основных данных
    public void UpdateDetails(Name name, string description, decimal defaultPrice, string imageUrl)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (defaultPrice <= 0)
            throw new ArgumentException("Default price must be positive", nameof(defaultPrice));

        Name = name;
        Description = description ?? string.Empty;
        DefaultPrice = defaultPrice;
        ImageUrl = imageUrl ?? ImageUrl;
    }

    public void UpdateSize(PizzaSize newSize)
    {
        Size = newSize ?? throw new ArgumentNullException(nameof(newSize));
    }

    public void UpdateCrust(CrustType newCrust)
    {
        Crust = newCrust;
    }

    // Вычисление полной цены с учетом ингредиентов
    public decimal CalculateTotalPrice(IEnumerable<Ingredient> allIngredients)
    {
        var ingredientsPrice = _standardIngredients
            .Join(allIngredients,
                pi => pi.IngredientId,
                i => i.Id,
                (pi, i) => i.BasePrice * pi.Quantity)
            .Sum();

        return DefaultPrice + ingredientsPrice;
    }
}
