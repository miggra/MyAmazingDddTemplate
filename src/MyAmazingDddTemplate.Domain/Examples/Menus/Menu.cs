using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Menus;

public class Menu : Entity<Guid>
{
    private readonly List<MenuItem> _menuItems = [];
    public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();
    public IEnumerable<IGrouping<MenuCategory, MenuItem>> ItemsByCategory => 
        _menuItems.GroupBy(item => item.Category);

    public Dictionary<MenuCategory, List<MenuItem>> GetCategorizedItems()
    {
        return _menuItems
            .GroupBy(item => item.Category)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
    public void AddMenuItem(MenuItem menuItem)
    {
        _menuItems.Add(menuItem);
    }
    public void RemoveMenuItem(MenuItem menuItem)
    {
        _menuItems.Remove(menuItem);
    }
}