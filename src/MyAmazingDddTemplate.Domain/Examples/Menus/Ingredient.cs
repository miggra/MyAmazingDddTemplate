using MyAmazingDddTemplate.Domain.Abstractions;

namespace MyAmazingDddTemplate.Domain.Examples.Menus;

public class Ingredient : Entity<Guid>
{
    public Ingredient(
        string name,
        decimal price,
        decimal description,
        string imageUrl) : base(Guid.NewGuid())
    {
        Name = name;
        Price = price;
        Description = description;
        ImageUrl = imageUrl;
    }

    public string Name {get; private set;}
    public decimal Price {get; private set;}
    public decimal Description {get; private set;}
    public string ImageUrl {get; private set;}
}