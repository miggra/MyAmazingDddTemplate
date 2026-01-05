namespace MyAmazingDddTemplate.Domain.Examples.Menus;

public enum MenuCategory
{
    Pizza = 1,
    Salad = 2,
    Drink = 3,
    Dessert = 4
}
public static class MenuCategoryLanguage
{
    public static string ToString(this MenuCategory menuCategory)
    {
        return menuCategory switch
        {
            MenuCategory.Pizza => "Пицца",
            MenuCategory.Salad => "Салаты",
            MenuCategory.Drink => "Напитки",
            MenuCategory.Dessert => "Десерты",
            _ => throw new ArgumentOutOfRangeException(nameof(menuCategory), $"Not expected menuCategory value: {menuCategory}"),
        };
    }
}
