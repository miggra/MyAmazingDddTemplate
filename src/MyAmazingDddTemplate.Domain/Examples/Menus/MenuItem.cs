using MyAmazingDddTemplate.Domain.Abstractions;
using MyAmazingDddTemplate.Domain.Examples.Shared;

namespace MyAmazingDddTemplate.Domain.Examples.Menus;

/// <summary>
/// Каталожная сущность (Catalog Entity) - элемент меню
/// Базовый класс для всех блюд в меню
/// </summary>
public abstract class MenuItem : Entity<Guid>
{
    // Защищенный конструктор без параметров для инфраструктуры
    protected MenuItem() : base() { }

    public Name Name { get; protected set; } = null!;
    public abstract MenuCategory Category { get; }
    public decimal DefaultPrice { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public string ImageUrl { get; protected set; } = string.Empty;
}
