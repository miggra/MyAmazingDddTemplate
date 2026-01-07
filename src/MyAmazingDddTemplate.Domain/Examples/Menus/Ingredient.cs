using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Menus;

/// <summary>
/// Справочная сущность (Reference Data Entity) - ингредиент для блюд
/// Может изменяться со временем, имеет собственный жизненный цикл
/// </summary>
public class Ingredient : Entity<Guid>
{
    // Приватный конструктор без параметров для инфраструктуры
    private Ingredient() : base() { }
    
    private Ingredient(
        Name name,
        decimal basePrice,
        string description,
        string imageUrl) : base(Guid.NewGuid())
    {
        Name = name;
        BasePrice = basePrice;
        Description = description;
        ImageUrl = imageUrl;
        IsActive = true;
    }

    public Name Name { get; private set; }
    public decimal BasePrice { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public bool IsActive { get; private set; }

    // Фабричный метод
    public static Ingredient Create(Name name, decimal basePrice, string description, string imageUrl)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        
        if (basePrice < 0)
            throw new ArgumentException("Base price cannot be negative", nameof(basePrice));
        
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty", nameof(imageUrl));
        
        return new Ingredient(name, basePrice, description ?? string.Empty, imageUrl);
    }

    // Бизнес-методы
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));
        
        BasePrice = newPrice;
    }

    public void UpdateDetails(Name name, string description, string imageUrl)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty", nameof(imageUrl));
        
        Name = name;
        Description = description ?? string.Empty;
        ImageUrl = imageUrl;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}